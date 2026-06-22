using ForumGuard.Domain.Enums;

namespace ForumGuard.Application.Comments;

/// <summary>
/// Conveys the outcome of a successful comment submission (SDD-FORUM-001).
/// <para>See <see cref="Interfaces.ICommentSubmissionService"/> and <see cref="SubmitCommentRequest"/>.</para>
/// </summary>
/// <param name="CommentId">The identifier of the created comment.</param>
/// <param name="Status">The terminal or queued status the comment reached: Published or FlaggedForReview.</param>
public sealed record SubmitCommentResult(Guid CommentId, CommentStatus Status);
