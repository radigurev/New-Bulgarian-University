namespace MeepleNight.Services.Dtos;

public class SessionDto
{
    public Guid Id { get; set; }
    public Guid GameNightId { get; set; }
    public Guid GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; }
    public int? DurationMinutes { get; set; }
    public string? WinnerNote { get; set; }
    public bool? IsCooperativeWin { get; set; }
    public List<SessionPlayerDto> Players { get; set; } = new();
}

public class SessionPlayerDto
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal? Score { get; set; }
    public int Placement { get; set; }
}

public class LogSessionRequest
{
    public Guid GameNightId { get; set; }
    public Guid GameId { get; set; }
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public int? DurationMinutes { get; set; }
    public string? WinnerNote { get; set; }
    public bool? IsCooperativeWin { get; set; }
    public List<LogSessionPlayer> Players { get; set; } = new();
}

public class LogSessionPlayer
{
    public Guid UserId { get; set; }
    public decimal? Score { get; set; }
    public int Placement { get; set; }
}
