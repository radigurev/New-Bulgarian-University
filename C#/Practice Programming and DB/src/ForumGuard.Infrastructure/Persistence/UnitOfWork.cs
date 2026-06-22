using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Infrastructure.Persistence;

/// <summary>
/// Implements the commit boundary by delegating to <see cref="ForumGuardDbContext.SaveChangesAsync(CancellationToken)"/>.
/// <para>See SDD-FORUM-022 (B-10, B-23). Concurrency and update exceptions are not swallowed; they
/// propagate so callers can map them to user-facing outcomes.</para>
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ForumGuardDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The shared database context.</param>
    public UnitOfWork(ForumGuardDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
