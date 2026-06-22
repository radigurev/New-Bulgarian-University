using ForumGuard.Domain.Entities;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Selects the moderation decisions recorded against a specific comment, ordered by
/// <see cref="ModerationDecision.DecidedAtUtc"/> ascending.
/// <para>See SDD-FORUM-022 (B-17). Backs <c>IModerationDecisionRepository.GetByCommentIdAsync</c>.</para>
/// </summary>
public sealed class ModerationDecisionsByCommentSpecification : Specification<ModerationDecision>
{
    /// <summary>
    /// Initializes the specification scoped to a comment.
    /// </summary>
    /// <param name="commentId">The identifier of the comment.</param>
    public ModerationDecisionsByCommentSpecification(Guid commentId)
        : base(decision => decision.CommentId == commentId)
    {
        ApplyOrderBy(decision => decision.DecidedAtUtc);
    }
}
