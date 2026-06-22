using FluentValidation;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace MeepleNight.Services;

public sealed class SessionService : ISessionService
{
    private readonly ISessionRepository _sessions;
    private readonly IGameNightRepository _nights;
    private readonly IGameRepository _games;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IValidator<LogSessionRequest> _validator;
    private readonly ILogger<SessionService> _logger;

    public SessionService(
        ISessionRepository sessions,
        IGameNightRepository nights,
        IGameRepository games,
        ICurrentUserAccessor currentUser,
        IValidator<LogSessionRequest> validator,
        ILogger<SessionService> logger)
    {
        _sessions = sessions;
        _nights = nights;
        _games = games;
        _currentUser = currentUser;
        _validator = validator;
        _logger = logger;
    }

    private Guid RequireUser() =>
        _currentUser.UserId ?? throw new InvalidOperationException("This call requires an authenticated user.");

    public async Task<Result<Guid>> LogAsync(LogSessionRequest request, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return Result<Guid>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        GameNight? night = await _nights.GetWithDetailsAsync(request.GameNightId, ct);
        if (night == null) return Result<Guid>.NotFound();
        if (night.HostUserId != userId) return Result<Guid>.Forbidden();

        if (night.Status == GameNightStatus.Cancelled)
        {
            return Result<Guid>.Conflict("Cannot log sessions on a cancelled night.");
        }

        if (request.StartedAtUtc < night.ScheduledForUtc.AddHours(-12))
        {
            return Result<Guid>.Fail("Session start is too far before the scheduled night start.");
        }

        Game? game = await _games.GetByIdAsync(request.GameId, includeInactive: true, ct);
        if (game == null) return Result<Guid>.Fail("Selected game not found.");

        var attendeeIds = night.Invitations
            .Where(i => i.Status == InvitationStatus.Accepted)
            .Select(i => i.InviteeUserId)
            .ToHashSet();

        foreach (LogSessionPlayer p in request.Players)
        {
            if (!attendeeIds.Contains(p.UserId) && p.UserId != night.HostUserId)
            {
                return Result<Guid>.Fail("All session players must be accepted attendees of the night.");
            }
        }

        // Cooperative game requires the IsCooperativeWin flag
        if (game.Category == GameCategory.Cooperative && request.IsCooperativeWin == null)
        {
            return Result<Guid>.Fail("Cooperative games require the cooperative-win flag.");
        }

        // Non-cooperative requires at least one Placement = 1
        if (game.Category != GameCategory.Cooperative && !request.Players.Any(p => p.Placement == 1))
        {
            return Result<Guid>.Fail("At least one player must finish in 1st place.");
        }

        Session session = new()
        {
            GameNightId = night.Id,
            GameId = request.GameId,
            StartedAtUtc = request.StartedAtUtc,
            DurationMinutes = request.DurationMinutes,
            WinnerNote = request.WinnerNote,
            IsCooperativeWin = game.Category == GameCategory.Cooperative ? request.IsCooperativeWin : null
        };

        foreach (LogSessionPlayer p in request.Players)
        {
            session.Players.Add(new SessionPlayer
            {
                UserId = p.UserId,
                Score = p.Score,
                Placement = p.Placement
            });
        }

        await _sessions.AddAsync(session, ct);

        // First session transitions a Planned night to Completed
        if (night.Status == GameNightStatus.Planned && !night.Sessions.Any())
        {
            night.Status = GameNightStatus.Completed;
            _nights.Update(night);
        }

        await _sessions.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} logged session {SessionId} on night {NightId}", userId, session.Id, night.Id);
        return Result<Guid>.Ok(session.Id);
    }

    public async Task<Result> DeleteAsync(Guid sessionId, CancellationToken ct = default)
    {
        Guid userId = RequireUser();

        Session? session = await _sessions.GetWithPlayersAsync(sessionId, ct);
        if (session == null) return Result.NotFound();

        GameNight? night = await _nights.GetWithDetailsAsync(session.GameNightId, ct);
        if (night == null) return Result.NotFound();
        if (night.HostUserId != userId) return Result.Forbidden();

        // Within 7 days of the night's scheduled time
        if (DateTime.UtcNow > night.ScheduledForUtc.AddDays(7))
        {
            return Result.Conflict("Sessions become read-only 7 days after the night.");
        }

        _sessions.Remove(session);

        // If the deleted one was the last session, revert night to Planned
        bool wasLast = night.Sessions.Count == 1 && night.Sessions.First().Id == sessionId;
        if (wasLast && night.Status == GameNightStatus.Completed)
        {
            night.Status = GameNightStatus.Planned;
            _nights.Update(night);
        }

        await _sessions.SaveChangesAsync(ct);

        _logger.LogInformation("Host {UserId} deleted session {SessionId} on night {NightId}", userId, sessionId, night.Id);
        return Result.Ok();
    }
}
