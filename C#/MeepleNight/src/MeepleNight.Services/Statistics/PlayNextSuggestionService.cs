using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace MeepleNight.Services.Statistics;

public sealed class PlayNextSuggestionService : IPlayNextSuggestionService
{
    private const double NeutralPrior = 0.5;
    private const int CooldownCapDays = 90;
    private const double DiversityScoreEpsilon = 0.05;

    private readonly IGameRepository _games;
    private readonly ISessionRepository _sessions;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly ILogger<PlayNextSuggestionService> _logger;

    public PlayNextSuggestionService(
        IGameRepository games,
        ISessionRepository sessions,
        ICurrentUserAccessor currentUser,
        ILogger<PlayNextSuggestionService> logger)
    {
        _games = games;
        _sessions = sessions;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<PlayNextResult> SuggestAsync(int playerCount, CancellationToken ct = default)
    {
        try
        {
            int p = Math.Clamp(playerCount, 1, 20);

            List<Game> eligible = await _games.ListActiveAsync(
                category: null,
                playerCount: p,
                maxDurationMinutes: null,
                search: null,
                ct);

            if (eligible.Count == 0)
            {
                return new PlayNextResult { Reason = "no_eligible_games" };
            }

            Guid? userId = _currentUser.UserId;
            List<Session> userSessions = userId.HasValue
                ? await _sessions.GetForUserAsync(userId.Value, ct)
                : new List<Session>();

            // Build per-game stats for the user
            var statsByGame = userSessions
                .Where(s => s.Game != null)
                .GroupBy(s => s.GameId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        int plays = g.Count();
                        int wins = 0;
                        DateTime lastPlayed = DateTime.MinValue;
                        foreach (Session s in g)
                        {
                            SessionPlayer? me = s.Players.FirstOrDefault(pl => pl.UserId == userId);
                            if (me != null)
                            {
                                if (s.Game!.Category == GameCategory.Cooperative)
                                {
                                    if (s.IsCooperativeWin == true) wins++;
                                }
                                else if (me.Placement == 1) wins++;
                            }
                            if (s.StartedAtUtc > lastPlayed) lastPlayed = s.StartedAtUtc;
                        }
                        return (Plays: plays, Wins: wins, LastPlayed: lastPlayed);
                    });

            DateTime now = DateTime.UtcNow;
            List<(Game Game, double Score, string Reason)> scored = new();

            foreach (Game g in eligible)
            {
                statsByGame.TryGetValue(g.Id, out var stat);
                double affinity = stat.Plays >= 3
                    ? (double)stat.Wins / stat.Plays
                    : NeutralPrior;

                double cooldown = stat.Plays == 0
                    ? 1.0
                    : Math.Min((now - stat.LastPlayed).TotalDays, CooldownCapDays) / CooldownCapDays;

                double score = (affinity * 0.6) + (cooldown * 0.4);

                string reason = stat.Plays == 0
                    ? "New to you"
                    : stat.Plays >= 3 && affinity > 0.6
                        ? $"Strong record ({(int)Math.Round(affinity * 100)}%)"
                        : $"Haven't played in {(int)(now - stat.LastPlayed).TotalDays} days";

                scored.Add((g, score, reason));
            }

            // Diversity demotion: when two scores are within epsilon AND same category, prefer the higher
            scored = scored
                .OrderByDescending(t => t.Score)
                .ThenBy(t => t.Game.Title)
                .ToList();

            var demoted = new HashSet<Guid>();
            for (int i = 0; i < scored.Count; i++)
            {
                if (demoted.Contains(scored[i].Game.Id)) continue;
                for (int j = i + 1; j < scored.Count; j++)
                {
                    if (demoted.Contains(scored[j].Game.Id)) continue;
                    if (scored[i].Game.Category == scored[j].Game.Category
                        && Math.Abs(scored[i].Score - scored[j].Score) <= DiversityScoreEpsilon)
                    {
                        demoted.Add(scored[j].Game.Id);
                    }
                }
            }

            var top5 = scored
                .Where(t => !demoted.Contains(t.Game.Id))
                .Take(5)
                .Select(t => new PlayNextSuggestionDto
                {
                    GameId = t.Game.Id,
                    Title = t.Game.Title,
                    Category = t.Game.Category,
                    MinPlayers = t.Game.MinPlayers,
                    MaxPlayers = t.Game.MaxPlayers,
                    Score = Math.Round(t.Score, 3),
                    Reason = t.Reason
                })
                .ToList();

            return new PlayNextResult { Suggestions = top5 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PlayNext suggestion engine failed");
            return new PlayNextResult { Reason = "service_error" };
        }
    }
}
