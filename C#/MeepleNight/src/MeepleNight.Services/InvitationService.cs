using AutoMapper;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace MeepleNight.Services;

public sealed class InvitationService : IInvitationService
{
    private readonly IInvitationRepository _invitations;
    private readonly IGameNightRepository _nights;
    private readonly IUserRepository _users;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IMapper _mapper;
    private readonly ILogger<InvitationService> _logger;

    public InvitationService(
        IInvitationRepository invitations,
        IGameNightRepository nights,
        IUserRepository users,
        ICurrentUserAccessor currentUser,
        IMapper mapper,
        ILogger<InvitationService> logger)
    {
        _invitations = invitations;
        _nights = nights;
        _users = users;
        _currentUser = currentUser;
        _mapper = mapper;
        _logger = logger;
    }

    private Guid RequireUser() =>
        _currentUser.UserId ?? throw new InvalidOperationException("This call requires an authenticated user.");

    public async Task<List<InboxInvitationDto>> GetInboxAsync(CancellationToken ct = default)
    {
        Guid userId = RequireUser();
        List<Invitation> rows = await _invitations.GetInboxForUserAsync(userId, ct);
        List<InboxInvitationDto> dtos = _mapper.Map<List<InboxInvitationDto>>(rows);

        // resolve host display names
        var hostIds = rows.Where(r => r.GameNight != null).Select(r => r.GameNight!.HostUserId).Distinct().ToList();
        var hosts = await _users.GetByIdsAsync(hostIds, ct);
        var hostMap = hosts.ToDictionary(h => h.Id, h => h.DisplayName);

        for (int i = 0; i < rows.Count; i++)
        {
            Guid? hostId = rows[i].GameNight?.HostUserId;
            if (hostId != null && hostMap.TryGetValue(hostId.Value, out string? name))
            {
                dtos[i].HostDisplayName = name;
            }
        }

        return dtos;
    }

    public Task<int> GetPendingCountAsync(CancellationToken ct = default)
    {
        Guid userId = RequireUser();
        return _invitations.PendingCountForUserAsync(userId, ct);
    }

    public async Task<Result<int>> SendAsync(SendInvitationsRequest request, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        GameNight? night = await _nights.GetWithDetailsAsync(request.GameNightId, ct);
        if (night == null) return Result<int>.NotFound();
        if (night.HostUserId != userId) return Result<int>.Forbidden();
        if (night.Status != GameNightStatus.Planned)
        {
            return Result<int>.Conflict("Cannot invite users to a night that is not Planned.");
        }

        var distinctIds = request.InviteeUserIds.Distinct().ToList();
        if (distinctIds.Any(id => id == userId))
        {
            return Result<int>.Fail("You can't invite yourself.");
        }

        var existingInviteeIds = night.Invitations.Select(i => i.InviteeUserId).ToHashSet();
        var newIds = distinctIds.Where(id => !existingInviteeIds.Contains(id)).ToList();

        var users = await _users.GetByIdsAsync(newIds, ct);
        if (users.Count != newIds.Count)
        {
            return Result<int>.Fail("One of the selected users no longer exists.");
        }

        var invitations = newIds.Select(invId => new Invitation
        {
            GameNightId = night.Id,
            InviteeUserId = invId,
            InvitedByUserId = userId,
            Status = InvitationStatus.Pending
        }).ToList();

        await _invitations.AddRangeAsync(invitations, ct);
        await _invitations.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} sent {Count} invitations for night {NightId}", userId, invitations.Count, night.Id);
        return Result<int>.Ok(invitations.Count);
    }

    public async Task<Result> RespondAsync(RespondToInvitationRequest request, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        if (request.Response != InvitationStatus.Accepted && request.Response != InvitationStatus.Declined)
        {
            return Result.Fail("Invalid response value.");
        }

        Invitation? invitation = await _invitations.GetByIdAsync(request.InvitationId, ct);
        if (invitation == null) return Result.NotFound();
        if (invitation.InviteeUserId != userId) return Result.Forbidden();

        GameNight? night = await _nights.GetByIdAsync(invitation.GameNightId, ct);
        if (night == null) return Result.NotFound();
        if (night.Status != GameNightStatus.Planned || night.ScheduledForUtc <= DateTime.UtcNow)
        {
            return Result.Conflict("This night is no longer accepting RSVPs.");
        }

        if (invitation.Status == InvitationStatus.Revoked || invitation.Status == InvitationStatus.Removed)
        {
            return Result.Conflict("This invitation cannot be changed.");
        }

        if (invitation.Status == request.Response)
        {
            return Result.Ok();   // idempotent
        }

        invitation.Status = request.Response;
        invitation.RespondedAtUtc = DateTime.UtcNow;
        await _invitations.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} responded {Response} to invitation {InvitationId}",
            userId, request.Response, invitation.Id);
        return Result.Ok();
    }

    public async Task<Result> RevokeAsync(Guid invitationId, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        Invitation? invitation = await _invitations.GetByIdAsync(invitationId, ct);
        if (invitation == null) return Result.NotFound();

        GameNight? night = await _nights.GetByIdAsync(invitation.GameNightId, ct);
        if (night == null) return Result.NotFound();
        if (night.HostUserId != userId) return Result.Forbidden();

        if (invitation.Status != InvitationStatus.Pending)
        {
            return Result.Conflict("Only pending invitations can be revoked.");
        }

        invitation.Status = InvitationStatus.Revoked;
        invitation.RespondedAtUtc = DateTime.UtcNow;
        await _invitations.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} revoked invitation {InvitationId}", userId, invitation.Id);
        return Result.Ok();
    }
}
