using MeepleNight.Domain.Entities;

namespace MeepleNight.Domain.Interfaces;

public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<Session?> GetWithPlayersAsync(Guid id, CancellationToken ct = default);

    Task<List<Session>> GetForNightAsync(Guid gameNightId, CancellationToken ct = default);

    Task<List<Session>> GetForUserAsync(Guid userId, CancellationToken ct = default);

    Task AddAsync(Session session, CancellationToken ct = default);

    void Remove(Session session);

    Task SaveChangesAsync(CancellationToken ct = default);
}
