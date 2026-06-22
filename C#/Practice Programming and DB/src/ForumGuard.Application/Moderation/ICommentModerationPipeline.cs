namespace ForumGuard.Application.Moderation;

/// <summary>
/// Defines the entry point that traverses the moderation Chain of Responsibility for a single comment.
/// <para>Invoked by the comment-submission flow (SDD-FORUM-001); maps onto a <see cref="ModerationVerdict"/>.</para>
/// <para>See <see cref="ICommentModerationHandler"/> and <see cref="CommentModerationContext"/>.</para>
/// </summary>
public interface ICommentModerationPipeline
{
    /// <summary>
    /// Evaluates the comment through the configured handler chain, failing safe to flagged on handler error.
    /// </summary>
    /// <param name="context">The comment under analysis.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The pipeline verdict; an empty chain yields a clean verdict with zero score.</returns>
    Task<ModerationVerdict> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default);
}
