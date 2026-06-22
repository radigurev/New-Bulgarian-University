using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;

namespace MeepleNight.Domain.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, bool includeInactive = false, CancellationToken ct = default);

    Task<List<Game>> ListActiveAsync(
        GameCategory? category,
        int? playerCount,
        int? maxDurationMinutes,
        string? search,
        CancellationToken ct = default);

    Task<List<Game>> ListAllForAdminAsync(string? search, CancellationToken ct = default);

    Task<bool> TitleExistsActiveAsync(string title, Guid? excludeId, CancellationToken ct = default);

    Task<int> SessionCountAsync(Guid gameId, CancellationToken ct = default);

    Task<int> SessionCountByUserAsync(Guid gameId, Guid userId, CancellationToken ct = default);

    Task AddAsync(Game game, CancellationToken ct = default);
    void Update(Game game);
    Task SaveChangesAsync(CancellationToken ct = default);
}
