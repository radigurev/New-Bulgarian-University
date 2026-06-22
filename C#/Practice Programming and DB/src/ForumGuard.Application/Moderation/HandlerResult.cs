namespace ForumGuard.Application.Moderation;

/// <summary>
/// Represents a single handler's evaluation: whether it flags or passes, and the toxic-score it observed.
/// <para>See <see cref="ICommentModerationHandler"/> and <see cref="HandlerDecision"/>.</para>
/// </summary>
/// <param name="Decision">Whether the handler flags (short-circuits) or passes the comment on.</param>
/// <param name="Score">The toxic-score the handler's analyzer produced, in the range <c>[0.0, 1.0]</c>.</param>
public sealed record HandlerResult(HandlerDecision Decision, float Score)
{
    /// <summary>
    /// Gets a value indicating whether the handler flagged the comment.
    /// </summary>
    public bool IsFlag => Decision == HandlerDecision.Flag;

    /// <summary>
    /// Creates a pass result carrying the observed toxic-score.
    /// </summary>
    /// <param name="score">The toxic-score the handler observed.</param>
    /// <returns>A pass result.</returns>
    public static HandlerResult Pass(float score) => new(HandlerDecision.Pass, score);

    /// <summary>
    /// Creates a flag result carrying the toxic-score that triggered the flag.
    /// </summary>
    /// <param name="score">The toxic-score that triggered the flag.</param>
    /// <returns>A flag result.</returns>
    public static HandlerResult Flag(float score) => new(HandlerDecision.Flag, score);
}
