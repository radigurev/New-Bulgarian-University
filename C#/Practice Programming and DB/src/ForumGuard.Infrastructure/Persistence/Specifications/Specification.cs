using System.Linq.Expressions;
using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Provides a reusable base implementation of <see cref="ISpecification{T}"/> that concrete
/// specifications configure through protected builder methods.
/// <para>See SDD-FORUM-022. Holds the criteria, includes, ordering and paging that the
/// <see cref="SpecificationEvaluator{T}"/> composes onto an <see cref="IQueryable{T}"/>.</para>
/// </summary>
/// <typeparam name="T">The entity type the specification targets.</typeparam>
public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<OrderExpression<T>> _thenByExpressions = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class with no criteria.
    /// </summary>
    protected Specification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class with the given criteria.
    /// </summary>
    /// <param name="criteria">The server-evaluable filter predicate.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        Criteria = criteria;
    }

    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes;

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <summary>
    /// Gets the secondary ordering keys applied after the primary <see cref="OrderBy"/> or
    /// <see cref="OrderByDescending"/>, in the order they were registered.
    /// </summary>
    public IReadOnlyList<OrderExpression<T>> ThenByExpressions => _thenByExpressions;

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <inheritdoc />
    public bool IsPagingEnabled { get; private set; }

    /// <summary>
    /// Sets the server-evaluable filter predicate for the specification.
    /// </summary>
    /// <param name="criteria">The filter predicate.</param>
    protected void ApplyCriteria(Expression<Func<T, bool>> criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        Criteria = criteria;
    }

    /// <summary>
    /// Registers a navigation property to eagerly include in the query.
    /// </summary>
    /// <param name="includeExpression">The navigation expression to include.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        ArgumentNullException.ThrowIfNull(includeExpression);
        _includes.Add(includeExpression);
    }

    /// <summary>
    /// Sets the ascending order-by expression.
    /// </summary>
    /// <param name="orderByExpression">The ascending order key selector.</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        ArgumentNullException.ThrowIfNull(orderByExpression);
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Sets the descending order-by expression.
    /// </summary>
    /// <param name="orderByDescendingExpression">The descending order key selector.</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        ArgumentNullException.ThrowIfNull(orderByDescendingExpression);
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Registers an ascending secondary ordering key applied after the primary order as a stable
    /// tiebreaker. A primary order (<see cref="ApplyOrderBy"/> or <see cref="ApplyOrderByDescending"/>)
    /// must be set before adding secondary orderings.
    /// </summary>
    /// <param name="thenByExpression">The ascending secondary key selector.</param>
    protected void ApplyThenBy(Expression<Func<T, object>> thenByExpression)
    {
        ArgumentNullException.ThrowIfNull(thenByExpression);
        _thenByExpressions.Add(new OrderExpression<T>(thenByExpression, isDescending: false));
    }

    /// <summary>
    /// Registers a descending secondary ordering key applied after the primary order as a stable
    /// tiebreaker. A primary order (<see cref="ApplyOrderBy"/> or <see cref="ApplyOrderByDescending"/>)
    /// must be set before adding secondary orderings.
    /// </summary>
    /// <param name="thenByDescendingExpression">The descending secondary key selector.</param>
    protected void ApplyThenByDescending(Expression<Func<T, object>> thenByDescendingExpression)
    {
        ArgumentNullException.ThrowIfNull(thenByDescendingExpression);
        _thenByExpressions.Add(new OrderExpression<T>(thenByDescendingExpression, isDescending: true));
    }

    /// <summary>
    /// Enables paging on the specification.
    /// </summary>
    /// <param name="skip">The number of rows to skip.</param>
    /// <param name="take">The number of rows to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
}
