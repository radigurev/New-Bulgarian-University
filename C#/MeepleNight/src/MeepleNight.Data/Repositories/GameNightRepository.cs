using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Data.Repositories;

public sealed class GameNightRepository : IGameNightRepository
{
    private readonly MeepleDbContext _db;

    public GameNightRepository(MeepleDbContext db) => _db = db;

    public Task<GameNight?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.GameNights.FirstOrDefaultAsync(n => n.Id == id, ct);

    public Task<GameNight?> GetWithDetailsAsync(Guid id, CancellationToken ct = default)
        => _db.GameNights
            .Include(n => n.Host)
            .Include(n => n.Candidates).ThenInclude(c => c.Game)
            .Include(n => n.Invitations).ThenInclude(i => i.Invitee)
            .Include(n => n.Sessions).ThenInclude(s => s.Game)
            .Include(n => n.Sessions).ThenInclude(s => s.Players).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(n => n.Id == id, ct);

    public Task<List<GameNight>> ListHostedByAsync(Guid hostUserId, CancellationToken ct = default)
        => _db.GameNights
            .Where(n => n.HostUserId == hostUserId)
            .OrderBy(n => n.ScheduledForUtc)
            .ToListAsync(ct);

    public Task<List<GameNight>> ListInvitedToAsync(Guid userId, CancellationToken ct = default)
        => _db.GameNights
            .Where(n => n.HostUserId != userId)
            .Where(n => n.Invitations.Any(i => i.InviteeUserId == userId
                && (i.Status == InvitationStatus.Accepted || i.Status == InvitationStatus.Pending)))
            .OrderBy(n => n.ScheduledForUtc)
            .ToListAsync(ct);

    public async Task AddAsync(GameNight night, CancellationToken ct = default)
        => await _db.GameNights.AddAsync(night, ct);

    public void Update(GameNight night) => _db.GameNights.Update(night);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
