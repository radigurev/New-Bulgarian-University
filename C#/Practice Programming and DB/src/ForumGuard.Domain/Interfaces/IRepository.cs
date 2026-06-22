namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines the generic CRUD and specification-query contract for an entity type.
/// <para>Mutating members never persist; committing is the responsibility of <see cref="IUnitOfWork"/>.</para>
/// </summary>
/// <typeparam name="T">The entity type the repository manages.</typeparam>
public interface IRepository<T>
    where T : class
{
    /// <summary>
    /// Retrieves an entity by its primary key, or <c>null</c> when no matching row exists.
    /// </summary>
    /// <param name="id">The primary key to look up.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The matching entity, or <c>null</c>.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all entities matching the supplied specification.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The matching entities.</returns>
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the single entity matching the supplied specification, or <c>null</c> when none matches.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The matching entity, or <c>null</c>.</returns>
    Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the count of entities matching the supplied specification.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The number of matching entities.</returns>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks the supplied entity for insertion. Does not persist.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(T entity);

    /// <summary>
    /// Tracks the supplied entity as modified. Does not persist.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(T entity);

    /// <summary>
    /// Tracks the supplied entity for deletion. Does not persist.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(T entity);
}
