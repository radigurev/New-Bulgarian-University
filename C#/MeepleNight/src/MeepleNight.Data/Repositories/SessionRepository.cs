using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Data.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly MeepleDbContext _db;

    public SessionRepository(MeepleDbContext db) => _db = db;

    public Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Sessions.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Session?> GetWithPlayersAsync(Guid id, CancellationToken ct = default)
        => _db.Sessions
            .Include(s => s.Game)
            .Include(s => s.Players).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<Session>> GetForNightAsync(Guid gameNightId, CancellationToken ct = default)
        => _db.Sessions
            .Where(s => s.GameNightId == gameNightId)
            .Include(s => s.Game)
            .Include(s => s.Players).ThenInclude(p => p.User)
            .OrderBy(s => s.StartedAtUtc)
            .ToListAsync(ct);

    public Task<List<Session>> GetForUserAsync(Guid userId, CancellationToken ct = default)
        => _db.Sessions
            .Where(s => s.Players.Any(p => p.UserId == userId))
            .Include(s => s.Game)
            .Include(s => s.GameNight)
            .Include(s => s.Players)
            .ToListAsync(ct);

    public async Task AddAsync(Session session, CancellationToken ct = default)
        => await _db.Sessions.AddAsync(session, ct);

    public void Remove(Session session) => _db.Sessions.Remove(session);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
