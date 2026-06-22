namespace ForumGuard.Application.Moderation;

/// <summary>
/// Defines a single link in the moderation Chain of Responsibility (SDD-FORUM-021).
/// <para>Each handler either flags a comment (short-circuiting the chain) or passes it to the next link.</para>
/// <para>See <see cref="ICommentModerationPipeline"/>, <see cref="HandlerResult"/>, and <see cref="CommentModerationContext"/>.</para>
/// </summary>
public interface ICommentModerationHandler
{
    /// <summary>
    /// Gets the stable name reported as the flagging handler when this link flags a comment.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the relative position of this handler within the configured chain; lower runs first.
    /// </summary>
    int Order { get; }

    /// <summary>
    /// Evaluates the comment and returns whether to flag (short-circuit) or pass to the next handler.
    /// </summary>
    /// <param name="context">The comment under analysis.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The handler's flag-or-pass decision and the toxic-score it observed.</returns>
    Task<HandlerResult> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default);
}
