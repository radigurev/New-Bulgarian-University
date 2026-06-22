using ForumGuard.Domain.Entities;

namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines moderation-decision-specific data access on top of the generic repository contract.
/// <para>See <see cref="ModerationDecision"/>.</para>
/// </summary>
public interface IModerationDecisionRepository : IRepository<ModerationDecision>
{
    /// <summary>
    /// Returns the moderation decisions recorded against the specified comment.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The decisions recorded for the comment.</returns>
    Task<IReadOnlyList<ModerationDecision>> GetByCommentIdAsync(Guid commentId, CancellationToken cancellationToken = default);
}
