using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Statistics;

public sealed class StatisticsService : IStatisticsService
{
    private readonly ISessionRepository _sessions;
    private readonly ICurrentUserAccessor _currentUser;

    public StatisticsService(ISessionRepository sessions, ICurrentUserAccessor currentUser)
    {
        _sessions = sessions;
        _currentUser = currentUser;
    }

    public async Task<StatisticsDashboardDto> GetForCurrentUserAsync(CancellationToken ct = default)
    {
        Guid userId = _currentUser.UserId
            ?? throw new InvalidOperationException("Statistics require an authenticated user.");

        List<Session> sessions = await _sessions.GetForUserAsync(userId, ct);
        StatisticsDashboardDto dto = new() { HasData = sessions.Count > 0 };
        if (!dto.HasData) return dto;

        // Total sessions and game nights
        dto.TotalSessionsPlayed = sessions.Count;
        dto.TotalGameNightsAttended = sessions.Select(s => s.GameNightId).Distinct().Count();

        // Win calculation
        int wins = 0;
        int countingForWinRate = 0;
        foreach (Session session in sessions)
        {
            SessionPlayer? me = session.Players.FirstOrDefault(p => p.UserId == userId);
            if (me == null) continue;

            if (session.Game?.Category == GameCategory.Cooperative)
            {
                if (session.IsCooperativeWin.HasValue)
                {
                    countingForWinRate++;
                    if (session.IsCooperativeWin.Value) wins++;
                }
            }
            else
            {
                countingForWinRate++;
                if (me.Placement == 1) wins++;
            }
        }
        dto.WinRatePercent = countingForWinRate == 0
            ? 0.0
            : Math.Round(wins * 100.0 / countingForWinRate, 1);

        // Per-game breakdown
        var byGame = sessions
            .Where(s => s.Game != null)
            .GroupBy(s => new { s.GameId, s.Game!.Title, s.Game.Category })
            .Select(g =>
            {
                int plays = g.Count();
                int gameWins = 0;
                DateTime lastPlayed = DateTime.MinValue;

                foreach (Session session in g)
                {
                    SessionPlayer? me = session.Players.First(p => p.UserId == userId);
                    if (g.Key.Category == GameCategory.Cooperative)
                    {
                        if (session.IsCooperativeWin == true) gameWins++;
                    }
                    else if (me.Placement == 1) gameWins++;

                    if (session.StartedAtUtc > lastPlayed) lastPlayed = session.StartedAtUtc;
                }

                return new GameStatsRowDto
                {
                    GameId = g.Key.GameId,
                    GameTitle = g.Key.Title,
                    Plays = plays,
                    Wins = gameWins,
                    WinRatePercent = plays == 0 ? 0.0 : Math.Round(gameWins * 100.0 / plays, 1),
                    LastPlayedAtUtc = lastPlayed
                };
            })
            .OrderByDescending(r => r.Plays)
            .ToList();

        dto.PerGameBreakdown = byGame;

        // Most-played and best-record
        GameStatsRowDto? mostPlayed = byGame.FirstOrDefault();
        if (mostPlayed != null)
        {
            dto.MostPlayedGameId = mostPlayed.GameId;
            dto.MostPlayedGameTitle = mostPlayed.GameTitle;
        }

        GameStatsRowDto? bestRecord = byGame
            .Where(r => r.Plays >= 3)
            .OrderByDescending(r => r.WinRatePercent)
            .FirstOrDefault();
        if (bestRecord != null)
        {
            dto.BestRecordGameTitle = bestRecord.GameTitle;
            dto.BestRecordWinRate = bestRecord.WinRatePercent;
        }

        // Frequent opponent
        var opponentCounts = sessions
            .SelectMany(s => s.Players)
            .Where(p => p.UserId != userId)
            .GroupBy(p => p.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count(), Name = g.First().User?.DisplayName })
            .OrderByDescending(g => g.Count)
            .FirstOrDefault();
        if (opponentCounts != null)
        {
            dto.FrequentOpponentDisplayName = opponentCounts.Name;
        }

        // Activity timeline (last 12 months, zero-padded)
        DateTime now = DateTime.UtcNow;
        var months = Enumerable.Range(0, 12)
            .Select(offset => now.AddMonths(-11 + offset))
            .Select(d => new MonthlyActivityDto { Year = d.Year, Month = d.Month, SessionCount = 0 })
            .ToList();

        foreach (Session session in sessions)
        {
            DateTime d = session.StartedAtUtc;
            MonthlyActivityDto? bucket = months.FirstOrDefault(m => m.Year == d.Year && m.Month == d.Month);
            if (bucket != null) bucket.SessionCount++;
        }
        dto.ActivityByMonth = months;

        return dto;
    }
}
