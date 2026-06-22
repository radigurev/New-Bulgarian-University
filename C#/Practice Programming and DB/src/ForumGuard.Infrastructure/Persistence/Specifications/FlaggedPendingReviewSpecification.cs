using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Selects the moderator queue: comments whose <see cref="Comment.Status"/> is
/// <see cref="CommentStatus.FlaggedForReview"/>, ordered by <see cref="Comment.CreatedAtUtc"/>
/// ascending (oldest first).
/// <para>See SDD-FORUM-022 (B-14). Excludes every other status.</para>
/// </summary>
public sealed class FlaggedPendingReviewSpecification : Specification<Comment>
{
    /// <summary>
    /// Initializes the specification matching only flagged-for-review comments, oldest first.
    /// </summary>
    public FlaggedPendingReviewSpecification()
        : base(comment => comment.Status == CommentStatus.FlaggedForReview)
    {
        ApplyOrderBy(comment => comment.CreatedAtUtc);
    }
}
