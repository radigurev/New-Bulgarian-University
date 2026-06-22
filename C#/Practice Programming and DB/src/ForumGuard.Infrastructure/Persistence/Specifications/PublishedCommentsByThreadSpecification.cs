using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Selects the publicly visible comments for a thread: rows for the given thread whose
/// <see cref="Comment.Status"/> is <see cref="CommentStatus.Published"/> or
/// <see cref="CommentStatus.ApprovedByModerator"/>, ordered by <see cref="Comment.PublishedAtUtc"/>
/// ascending with stable tiebreakers.
/// <para>See SDD-FORUM-022 (B-15). Excludes the hidden statuses. Falls back to
/// <see cref="Comment.CreatedAtUtc"/> and then <see cref="Comment.Id"/> so ordering stays fully
/// deterministic when <see cref="Comment.PublishedAtUtc"/> is null or tied.</para>
/// </summary>
public sealed class PublishedCommentsByThreadSpecification : Specification<Comment>
{
    /// <summary>
    /// Initializes the specification matching the publicly visible comments of a thread.
    /// </summary>
    /// <param name="threadId">The identifier of the thread to scope to.</param>
    public PublishedCommentsByThreadSpecification(Guid threadId)
        : base(comment => comment.ThreadId == threadId
            && (comment.Status == CommentStatus.Published
                || comment.Status == CommentStatus.ApprovedByModerator))
    {
        ApplyOrderBy(comment => comment.PublishedAtUtc!);
        ApplyThenBy(comment => comment.CreatedAtUtc);
        ApplyThenBy(comment => comment.Id);
    }
}
