using MeepleNight.Domain.Entities;

namespace MeepleNight.Domain.Interfaces;

public interface IGameNightRepository
{
    Task<GameNight?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<GameNight?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);

    Task<List<GameNight>> ListHostedByAsync(Guid hostUserId, CancellationToken ct = default);

    Task<List<GameNight>> ListInvitedToAsync(Guid userId, CancellationToken ct = default);

    Task AddAsync(GameNight night, CancellationToken ct = default);

    void Update(GameNight night);

    Task SaveChangesAsync(CancellationToken ct = default);
}
