using AutoMapper;
using FluentValidation;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace MeepleNight.Services;

public sealed class GameNightService : IGameNightService
{
    private readonly IGameNightRepository _nights;
    private readonly IGameRepository _games;
    private readonly IInvitationRepository _invitations;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateGameNightRequest> _createValidator;
    private readonly IValidator<UpdateGameNightRequest> _updateValidator;
    private readonly ILogger<GameNightService> _logger;

    public GameNightService(
        IGameNightRepository nights,
        IGameRepository games,
        IInvitationRepository invitations,
        ICurrentUserAccessor currentUser,
        IMapper mapper,
        IValidator<CreateGameNightRequest> createValidator,
        IValidator<UpdateGameNightRequest> updateValidator,
        ILogger<GameNightService> logger)
    {
        _nights = nights;
        _games = games;
        _invitations = invitations;
        _currentUser = currentUser;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private Guid RequireUser() =>
        _currentUser.UserId ?? throw new InvalidOperationException("This call requires an authenticated user.");

    public async Task<List<GameNightSummaryDto>> ListHostingAsync(CancellationToken ct = default)
    {
        Guid userId = RequireUser();
        List<GameNight> nights = await _nights.ListHostedByAsync(userId, ct);
        return MapWithCounts(nights);
    }

    public async Task<List<GameNightSummaryDto>> ListInvitedAsync(CancellationToken ct = default)
    {
        Guid userId = RequireUser();
        List<GameNight> nights = await _nights.ListInvitedToAsync(userId, ct);
        return MapWithCounts(nights);
    }

    private List<GameNightSummaryDto> MapWithCounts(List<GameNight> nights)
    {
        List<GameNightSummaryDto> list = _mapper.Map<List<GameNightSummaryDto>>(nights);
        for (int i = 0; i < nights.Count; i++)
        {
            list[i].AcceptedAttendeeCount = nights[i].Invitations.Count(x => x.Status == InvitationStatus.Accepted);
        }
        return list;
    }

    public async Task<GameNightDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        Guid userId = RequireUser();
        GameNight? night = await _nights.GetWithDetailsAsync(id, ct);
        if (night == null) return null;

        bool isHost = night.HostUserId == userId;
        bool isInvitee = night.Invitations.Any(i => i.InviteeUserId == userId);
        if (!isHost && !isInvitee && !_currentUser.IsAdmin) return null;

        GameNightDetailsDto dto = _mapper.Map<GameNightDetailsDto>(night);
        dto.ViewerIsHost = isHost;
        dto.ViewerIsAccepted = night.Invitations.Any(i =>
            i.InviteeUserId == userId && i.Status == InvitationStatus.Accepted);

        dto.CandidateGames = night.Candidates
            .OrderBy(c => c.Position)
            .Select(c => new CandidateGameDto
            {
                GameId = c.GameId,
                Title = c.Game?.Title ?? "(removed)",
                Category = c.Game?.Category ?? GameCategory.Other,
                MinPlayers = c.Game?.MinPlayers ?? 0,
                MaxPlayers = c.Game?.MaxPlayers ?? 0,
                Position = c.Position
            })
            .ToList();

        // hide pending invitations from non-host viewers
        IEnumerable<Invitation> visibleInvites = isHost
            ? night.Invitations
            : night.Invitations.Where(i => i.Status == InvitationStatus.Accepted);

        dto.Invitees = _mapper.Map<List<AttendeeDto>>(visibleInvites);

        dto.Sessions = night.Sessions
            .OrderBy(s => s.StartedAtUtc)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                GameNightId = s.GameNightId,
                GameId = s.GameId,
                GameTitle = s.Game?.Title ?? "(removed)",
                StartedAtUtc = s.StartedAtUtc,
                DurationMinutes = s.DurationMinutes,
                WinnerNote = s.WinnerNote,
                IsCooperativeWin = s.IsCooperativeWin,
                Players = s.Players.Select(p => new SessionPlayerDto
                {
                    UserId = p.UserId,
                    DisplayName = p.User?.DisplayName ?? "(unknown)",
                    Score = p.Score,
                    Placement = p.Placement
                }).OrderBy(p => p.Placement).ToList()
            })
            .ToList();

        return dto;
    }

    public async Task<Result<Guid>> CreateAsync(CreateGameNightRequest request, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return Result<Guid>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        // Confirm all candidate games are active
        foreach (Guid gameId in request.CandidateGameIds.Distinct())
        {
            Game? g = await _games.GetByIdAsync(gameId, includeInactive: false, ct);
            if (g == null)
            {
                return Result<Guid>.Fail("One of the selected games is no longer available.");
            }
        }

        GameNight night = new()
        {
            Title = request.Title,
            ScheduledForUtc = request.ScheduledForUtc,
            Location = request.Location,
            Notes = request.Notes,
            ExpectedPlayerCount = request.ExpectedPlayerCount,
            HostUserId = userId,
            Status = GameNightStatus.Planned
        };

        int position = 1;
        foreach (Guid gameId in request.CandidateGameIds.Distinct())
        {
            night.Candidates.Add(new GameNightCandidate
            {
                GameId = gameId,
                Position = position++
            });
        }

        // Auto-acceptance for the host
        night.Invitations.Add(new Invitation
        {
            InviteeUserId = userId,
            InvitedByUserId = userId,
            Status = InvitationStatus.Accepted,
            RespondedAtUtc = DateTime.UtcNow
        });

        await _nights.AddAsync(night, ct);
        await _nights.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} created game night {GameNightId}", userId, night.Id);
        return Result<Guid>.Ok(night.Id);
    }

    public async Task<Result> UpdateAsync(UpdateGameNightRequest request, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return Result.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        GameNight? night = await _nights.GetWithDetailsAsync(request.Id, ct);
        if (night == null) return Result.NotFound();
        if (night.HostUserId != userId) return Result.Forbidden();
        if (night.Status != GameNightStatus.Planned)
        {
            return Result.Conflict("This night is already completed or cancelled.");
        }

        night.Title = request.Title;
        night.ScheduledForUtc = request.ScheduledForUtc;
        night.Location = request.Location;
        night.Notes = request.Notes;
        night.ExpectedPlayerCount = request.ExpectedPlayerCount;
        night.UpdatedAtUtc = DateTime.UtcNow;

        // replace candidate list
        night.Candidates.Clear();
        int position = 1;
        foreach (Guid gameId in request.CandidateGameIds.Distinct())
        {
            night.Candidates.Add(new GameNightCandidate
            {
                GameNightId = night.Id,
                GameId = gameId,
                Position = position++
            });
        }

        _nights.Update(night);
        await _nights.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} updated game night {GameNightId}", userId, night.Id);
        return Result.Ok();
    }

    public async Task<Result> CancelAsync(Guid id, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        GameNight? night = await _nights.GetWithDetailsAsync(id, ct);
        if (night == null) return Result.NotFound();
        if (night.HostUserId != userId) return Result.Forbidden();
        if (night.Status != GameNightStatus.Planned)
        {
            return Result.Conflict("This night cannot be cancelled in its current status.");
        }

        night.Status = GameNightStatus.Cancelled;
        night.CancelledAtUtc = DateTime.UtcNow;

        // cascade-decline pending invitations
        foreach (Invitation pending in night.Invitations.Where(i => i.Status == InvitationStatus.Pending))
        {
            pending.Status = InvitationStatus.Revoked;
            pending.RespondedAtUtc = DateTime.UtcNow;
        }

        _nights.Update(night);
        await _nights.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} cancelled game night {GameNightId}", userId, night.Id);
        return Result.Ok();
    }
}
