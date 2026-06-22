using System.Linq.Expressions;

namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines a query specification describing criteria, includes, ordering, and paging for an entity type.
/// </summary>
/// <typeparam name="T">The entity type the specification targets.</typeparam>
public interface ISpecification<T>
{
    /// <summary>
    /// Gets the server-evaluable filter predicate, or <c>null</c> when no filtering is applied.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the navigation properties to eagerly include in the query.
    /// </summary>
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the ascending order-by expression, or <c>null</c> when not ordering ascending.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the descending order-by expression, or <c>null</c> when not ordering descending.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of rows to skip, or <c>null</c> when paging is not applied.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets the number of rows to take, or <c>null</c> when paging is not applied.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets a value indicating whether paging is applied to the query.
    /// </summary>
    bool IsPagingEnabled { get; }
}
