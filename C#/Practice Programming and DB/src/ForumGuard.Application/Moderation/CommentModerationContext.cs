namespace ForumGuard.Application.Moderation;

/// <summary>
/// Carries the input a single comment supplies to the moderation pipeline.
/// <para>See <see cref="ICommentModerationHandler"/> and <see cref="ICommentModerationPipeline"/>.</para>
/// </summary>
/// <param name="Body">The comment text to evaluate; a <c>null</c> value is treated as empty.</param>
/// <param name="CommentId">The optional identifier of the comment, carried for traceability.</param>
public sealed record CommentModerationContext(string? Body, Guid? CommentId = null)
{
    /// <summary>
    /// Gets the comment text with a <c>null</c> <see cref="Body"/> normalized to an empty string.
    /// </summary>
    public string SafeBody => Body ?? string.Empty;
}
