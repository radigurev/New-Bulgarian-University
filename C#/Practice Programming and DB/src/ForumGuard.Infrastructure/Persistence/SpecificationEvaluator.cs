using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Infrastructure.Persistence;

/// <summary>
/// Translates an <see cref="ISpecification{T}"/> into a composed <see cref="IQueryable{T}"/> by
/// applying its criteria, includes, ordering and paging server-side.
/// <para>See SDD-FORUM-022 (B-12, B-13). Criteria are never evaluated client-side.</para>
/// </summary>
/// <typeparam name="T">The entity type the evaluator queries.</typeparam>
public static class SpecificationEvaluator<T>
    where T : class
{
    /// <summary>
    /// Applies the supplied specification to the input query.
    /// </summary>
    /// <param name="inputQuery">The base query to compose onto.</param>
    /// <param name="specification">The specification to apply.</param>
    /// <returns>The query with criteria, includes, ordering and paging applied.</returns>
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        ArgumentNullException.ThrowIfNull(inputQuery);
        ArgumentNullException.ThrowIfNull(specification);

        IQueryable<T> query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = ApplyOrdering(query, specification);
        query = ApplyPaging(query, specification);

        return query;
    }

    private static IQueryable<T> ApplyOrdering(IQueryable<T> query, ISpecification<T> specification)
    {
        IOrderedQueryable<T>? orderedQuery = null;

        if (specification.OrderBy is not null)
        {
            orderedQuery = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            orderedQuery = query.OrderByDescending(specification.OrderByDescending);
        }

        if (orderedQuery is null)
        {
            return query;
        }

        return ApplySecondaryOrdering(orderedQuery, specification);
    }

    private static IQueryable<T> ApplySecondaryOrdering(IOrderedQueryable<T> orderedQuery, ISpecification<T> specification)
    {
        if (specification is not Specification<T> concreteSpecification)
        {
            return orderedQuery;
        }

        IOrderedQueryable<T> result = orderedQuery;
        foreach (OrderExpression<T> thenBy in concreteSpecification.ThenByExpressions)
        {
            result = thenBy.IsDescending
                ? result.ThenByDescending(thenBy.KeySelector)
                : result.ThenBy(thenBy.KeySelector);
        }

        return result;
    }

    private static IQueryable<T> ApplyPaging(IQueryable<T> query, ISpecification<T> specification)
    {
        if (!specification.IsPagingEnabled)
        {
            return query;
        }

        return query
            .Skip(specification.Skip ?? 0)
            .Take(specification.Take ?? int.MaxValue);
    }
}
