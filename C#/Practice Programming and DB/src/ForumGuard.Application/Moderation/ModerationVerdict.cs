using ForumGuard.Domain.Enums;

namespace ForumGuard.Application.Moderation;

/// <summary>
/// Represents the result the moderation pipeline returns for a single comment.
/// <para>A flagged verdict routes the comment to review; a clean verdict publishes it. See <see cref="ICommentModerationPipeline"/>.</para>
/// </summary>
public sealed record ModerationVerdict
{
    private ModerationVerdict(ToxicityLabel label, float score, string? flaggingHandlerName)
    {
        Label = label;
        Score = score;
        FlaggingHandlerName = flaggingHandlerName;
    }

    /// <summary>
    /// Gets the classification assigned to the comment.
    /// </summary>
    public ToxicityLabel Label { get; }

    /// <summary>
    /// Gets the toxic-score reported by the pipeline, in the inclusive range <c>[0.0, 1.0]</c>.
    /// </summary>
    public float Score { get; }

    /// <summary>
    /// Gets a value indicating whether the comment was flagged for review.
    /// </summary>
    public bool IsFlagged => Label == ToxicityLabel.Toxic;

    /// <summary>
    /// Gets the name of the handler that flagged the comment, or <c>null</c> when not flagged.
    /// </summary>
    public string? FlaggingHandlerName { get; }

    /// <summary>
    /// Creates a clean verdict carrying the supplied score and no flagging handler.
    /// </summary>
    /// <param name="score">The toxic-score reported by the last analyzing handler that ran.</param>
    /// <returns>A clean verdict with <see cref="IsFlagged"/> equal to <c>false</c>.</returns>
    public static ModerationVerdict Clean(float score) =>
        new(ToxicityLabel.Clean, ClampScore(score), null);

    /// <summary>
    /// Creates a flagged verdict naming the handler that flagged the comment.
    /// </summary>
    /// <param name="flaggingHandlerName">The name of the flagging handler; required.</param>
    /// <param name="score">The toxic-score that produced the flag.</param>
    /// <returns>A flagged verdict with <see cref="IsFlagged"/> equal to <c>true</c>.</returns>
    public static ModerationVerdict Flagged(string flaggingHandlerName, float score)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(flaggingHandlerName);
        return new ModerationVerdict(ToxicityLabel.Toxic, ClampScore(score), flaggingHandlerName);
    }

    private static float ClampScore(float score)
    {
        if (float.IsNaN(score) || score < 0.0f)
        {
            return 0.0f;
        }

        return score > 1.0f ? 1.0f : score;
    }
}
