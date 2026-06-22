using MeepleNight.Domain.Entities;

namespace MeepleNight.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<List<User>> SearchAsync(string query, int limit, CancellationToken ct = default);

    Task<List<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
}
