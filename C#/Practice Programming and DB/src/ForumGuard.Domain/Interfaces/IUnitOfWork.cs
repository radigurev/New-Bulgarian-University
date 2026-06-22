namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines the commit boundary that persists all tracked changes in a single transaction.
/// <para>See <see cref="IRepository{T}"/>.</para>
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists all tracked changes and returns the number of affected rows.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The number of state entries written to the underlying store.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
