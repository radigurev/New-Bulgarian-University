using MeepleNight.Domain.Enums;

namespace MeepleNight.Services.Dtos;

public class GameNightSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledForUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public Guid HostUserId { get; set; }
    public string HostDisplayName { get; set; } = string.Empty;
    public GameNightStatus Status { get; set; }
    public int AcceptedAttendeeCount { get; set; }
}

public class GameNightDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledForUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int ExpectedPlayerCount { get; set; }
    public Guid HostUserId { get; set; }
    public string HostDisplayName { get; set; } = string.Empty;
    public GameNightStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public bool ViewerIsHost { get; set; }
    public bool ViewerIsAccepted { get; set; }

    public List<CandidateGameDto> CandidateGames { get; set; } = new();
    public List<AttendeeDto> Invitees { get; set; } = new();
    public List<SessionDto> Sessions { get; set; } = new();
}

public class CandidateGameDto
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public GameCategory Category { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int Position { get; set; }
}

public class AttendeeDto
{
    public Guid InvitationId { get; set; }
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public InvitationStatus Status { get; set; }
}

public class CreateGameNightRequest
{
    public string Title { get; set; } = string.Empty;
    public DateTime ScheduledForUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int ExpectedPlayerCount { get; set; } = 4;
    public List<Guid> CandidateGameIds { get; set; } = new();
}

public class UpdateGameNightRequest : CreateGameNightRequest
{
    public Guid Id { get; set; }
}
