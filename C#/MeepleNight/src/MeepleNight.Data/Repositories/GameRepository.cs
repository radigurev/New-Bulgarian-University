using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Data.Repositories;

public sealed class GameRepository : IGameRepository
{
    private readonly MeepleDbContext _db;

    public GameRepository(MeepleDbContext db) => _db = db;

    public Task<Game?> GetByIdAsync(Guid id, bool includeInactive = false, CancellationToken ct = default)
    {
        IQueryable<Game> q = _db.Games;
        if (!includeInactive) q = q.Where(g => g.IsActive);
        return q.FirstOrDefaultAsync(g => g.Id == id, ct);
    }

    public async Task<List<Game>> ListActiveAsync(
        GameCategory? category,
        int? playerCount,
        int? maxDurationMinutes,
        string? search,
        CancellationToken ct = default)
    {
        IQueryable<Game> q = _db.Games.Where(g => g.IsActive);

        if (category.HasValue) q = q.Where(g => g.Category == category.Value);
        if (playerCount.HasValue) q = q.Where(g => g.MinPlayers <= playerCount.Value && g.MaxPlayers >= playerCount.Value);
        if (maxDurationMinutes.HasValue) q = q.Where(g => g.AverageDurationMinutes <= maxDurationMinutes.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim();
            q = q.Where(g => EF.Functions.Like(g.Title, $"%{s}%"));
        }

        return await q.OrderBy(g => g.Title).ToListAsync(ct);
    }

    public async Task<List<Game>> ListAllForAdminAsync(string? search, CancellationToken ct = default)
    {
        IQueryable<Game> q = _db.Games;
        if (!string.IsNullOrWhiteSpace(search))
        {
            string s = search.Trim();
            q = q.Where(g => EF.Functions.Like(g.Title, $"%{s}%"));
        }
        return await q.OrderBy(g => g.Title).ToListAsync(ct);
    }

    public async Task<bool> TitleExistsActiveAsync(string title, Guid? excludeId, CancellationToken ct = default)
    {
        return await _db.Games
            .Where(g => g.IsActive && g.Title == title)
            .Where(g => excludeId == null || g.Id != excludeId.Value)
            .AnyAsync(ct);
    }

    public Task<int> SessionCountAsync(Guid gameId, CancellationToken ct = default)
        => _db.Sessions.CountAsync(s => s.GameId == gameId, ct);

    public Task<int> SessionCountByUserAsync(Guid gameId, Guid userId, CancellationToken ct = default)
        => _db.SessionPlayers
            .Where(p => p.UserId == userId && p.Session!.GameId == gameId)
            .CountAsync(ct);

    public async Task AddAsync(Game game, CancellationToken ct = default)
        => await _db.Games.AddAsync(game, ct);

    public void Update(Game game) => _db.Games.Update(game);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
