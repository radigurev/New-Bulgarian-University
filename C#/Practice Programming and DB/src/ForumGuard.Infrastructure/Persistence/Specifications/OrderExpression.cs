using System.Linq.Expressions;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Represents a single secondary ordering key applied after the primary order of a specification,
/// together with the direction in which it is sorted.
/// <para>See SDD-FORUM-022 (B-15). Used to make specification ordering fully deterministic by
/// supplying stable tiebreakers via <see cref="SpecificationEvaluator{T}"/>.</para>
/// </summary>
/// <typeparam name="T">The entity type the ordering key selects from.</typeparam>
public sealed class OrderExpression<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderExpression{T}"/> class.
    /// </summary>
    /// <param name="keySelector">The key selector for the secondary ordering.</param>
    /// <param name="isDescending">A value indicating whether the key sorts descending.</param>
    public OrderExpression(Expression<Func<T, object>> keySelector, bool isDescending)
    {
        ArgumentNullException.ThrowIfNull(keySelector);
        KeySelector = keySelector;
        IsDescending = isDescending;
    }

    /// <summary>
    /// Gets the key selector for the secondary ordering.
    /// </summary>
    public Expression<Func<T, object>> KeySelector { get; }

    /// <summary>
    /// Gets a value indicating whether the key sorts descending.
    /// </summary>
    public bool IsDescending { get; }
}
