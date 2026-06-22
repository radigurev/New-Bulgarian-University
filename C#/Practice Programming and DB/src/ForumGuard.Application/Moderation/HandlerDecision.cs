namespace ForumGuard.Application.Moderation;

/// <summary>
/// Represents the two outcomes a moderation handler may produce: short-circuit (flag) or continue (pass).
/// <para>See <see cref="ICommentModerationHandler"/> and <see cref="HandlerResult"/>.</para>
/// </summary>
public enum HandlerDecision
{
    /// <summary>
    /// The handler found no toxic signal and passes control to the next handler in the chain.
    /// </summary>
    Pass,

    /// <summary>
    /// The handler flagged the comment as toxic and the chain short-circuits immediately.
    /// </summary>
    Flag
}
