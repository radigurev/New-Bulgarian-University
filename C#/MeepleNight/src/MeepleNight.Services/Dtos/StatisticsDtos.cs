namespace MeepleNight.Services.Dtos;

public class StatisticsDashboardDto
{
    public int TotalSessionsPlayed { get; set; }
    public int TotalGameNightsAttended { get; set; }
    public double WinRatePercent { get; set; }
    public string? MostPlayedGameTitle { get; set; }
    public Guid? MostPlayedGameId { get; set; }
    public string? BestRecordGameTitle { get; set; }
    public double? BestRecordWinRate { get; set; }
    public string? FrequentOpponentDisplayName { get; set; }
    public List<GameStatsRowDto> PerGameBreakdown { get; set; } = new();
    public List<MonthlyActivityDto> ActivityByMonth { get; set; } = new();
    public bool HasData { get; set; }
}

public class GameStatsRowDto
{
    public Guid GameId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public int Plays { get; set; }
    public int Wins { get; set; }
    public double WinRatePercent { get; set; }
    public DateTime LastPlayedAtUtc { get; set; }
}

public class MonthlyActivityDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int SessionCount { get; set; }
    public string Label => $"{Year:D4}-{Month:D2}";
}
