using ForumGuard.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides the generic EF Core implementation of <see cref="IRepository{T}"/>.
/// <para>See SDD-FORUM-022 (B-11, B-12). Mutating members track changes only; committing is the
/// responsibility of <see cref="IUnitOfWork"/>.</para>
/// </summary>
/// <typeparam name="T">The entity type the repository manages.</typeparam>
public class Repository<T> : IRepository<T>
    where T : class
{
    /// <summary>
    /// The shared database context.
    /// </summary>
    protected ForumGuardDbContext Context { get; }

    /// <summary>
    /// The entity set the repository operates on.
    /// </summary>
    protected DbSet<T> Set { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public Repository(ForumGuardDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
        Set = context.Set<T>();
    }

    /// <inheritdoc />
    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Set.FindAsync([id], cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<T?> SingleOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return await ApplySpecification(specification).CountAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public virtual void Add(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Add(entity);
    }

    /// <inheritdoc />
    public virtual void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Update(entity);
    }

    /// <inheritdoc />
    public virtual void Remove(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        Set.Remove(entity);
    }

    /// <summary>
    /// Composes the supplied specification onto the entity set's query.
    /// </summary>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The composed query.</returns>
    protected IQueryable<T> ApplySpecification(ISpecification<T> specification)
    {
        return SpecificationEvaluator<T>.GetQuery(Set.AsQueryable(), specification);
    }
}
