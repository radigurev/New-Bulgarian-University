using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Data.Repositories;

public sealed class InvitationRepository : IInvitationRepository
{
    private readonly MeepleDbContext _db;

    public InvitationRepository(MeepleDbContext db) => _db = db;

    public Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Invitations.FirstOrDefaultAsync(i => i.Id == id, ct);

    public Task<Invitation?> GetForUserAndNightAsync(Guid userId, Guid gameNightId, CancellationToken ct = default)
        => _db.Invitations.FirstOrDefaultAsync(
            i => i.InviteeUserId == userId && i.GameNightId == gameNightId, ct);

    public Task<List<Invitation>> GetForNightAsync(Guid gameNightId, CancellationToken ct = default)
        => _db.Invitations
            .Where(i => i.GameNightId == gameNightId)
            .Include(i => i.Invitee)
            .ToListAsync(ct);

    public Task<List<Invitation>> GetInboxForUserAsync(Guid userId, CancellationToken ct = default)
        => _db.Invitations
            .Where(i => i.InviteeUserId == userId)
            .Include(i => i.GameNight)
            .OrderBy(i => i.GameNight!.ScheduledForUtc)
            .ToListAsync(ct);

    public Task<int> PendingCountForUserAsync(Guid userId, CancellationToken ct = default)
        => _db.Invitations.CountAsync(
            i => i.InviteeUserId == userId
              && i.Status == InvitationStatus.Pending
              && i.GameNight!.ScheduledForUtc > DateTime.UtcNow,
            ct);

    public async Task AddRangeAsync(IEnumerable<Invitation> invitations, CancellationToken ct = default)
        => await _db.Invitations.AddRangeAsync(invitations, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);
}
