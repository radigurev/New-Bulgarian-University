using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly MeepleDbContext _db;

    public UserRepository(MeepleDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<List<User>> SearchAsync(string query, int limit, CancellationToken ct = default)
    {
        string q = (query ?? string.Empty).Trim();
        if (q.Length == 0)
        {
            return Task.FromResult(new List<User>());
        }

        return _db.Users
            .Where(u => EF.Functions.Like(u.DisplayName, $"%{q}%")
                     || EF.Functions.Like(u.Email!, $"%{q}%"))
            .OrderBy(u => u.DisplayName)
            .Take(limit)
            .ToListAsync(ct);
    }

    public Task<List<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        => _db.Users.Where(u => ids.Contains(u.Id)).ToListAsync(ct);
}
