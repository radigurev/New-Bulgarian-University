using MeepleNight.Domain.Entities;

namespace MeepleNight.Domain.Interfaces;

public interface IInvitationRepository
{
    Task<Invitation?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Invitation?> GetForUserAndNightAsync(Guid userId, Guid gameNightId, CancellationToken ct = default);

    Task<List<Invitation>> GetForNightAsync(Guid gameNightId, CancellationToken ct = default);

    Task<List<Invitation>> GetInboxForUserAsync(Guid userId, CancellationToken ct = default);

    Task<int> PendingCountForUserAsync(Guid userId, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<Invitation> invitations, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
