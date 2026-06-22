using ForumGuard.Application.Common;

namespace ForumGuard.Application.Comments.Interfaces;

/// <summary>
/// Defines the comment-submission use case: validate, create, analyze, and persist a comment (SDD-FORUM-001).
/// <para>See <see cref="SubmitCommentRequest"/> and <see cref="SubmitCommentResult"/>.</para>
/// </summary>
public interface ICommentSubmissionService
{
    /// <summary>
    /// Submits a comment: gates the submitter, validates input, routes the comment through the moderation pipeline, and persists the outcome.
    /// </summary>
    /// <param name="request">The submission input.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result with the created comment's status, or a typed failure.</returns>
    Task<Result<SubmitCommentResult>> SubmitCommentAsync(SubmitCommentRequest request, CancellationToken cancellationToken = default);
}
