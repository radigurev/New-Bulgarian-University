using ForumGuard.Domain.Enums;

namespace ForumGuard.Domain.Entities;

/// <summary>
/// Represents an audit record of a moderator's decision on a flagged comment.
/// <para>See <see cref="Comment"/> and <see cref="ModerationOutcome"/>.</para>
/// </summary>
public sealed class ModerationDecision
{
    /// <summary>
    /// The maximum permitted length of <see cref="Reason"/>.
    /// </summary>
    public const int MaxReasonLength = 500;

    private ModerationDecision()
    {
    }

    /// <summary>
    /// Creates a new moderation decision record.
    /// </summary>
    /// <param name="commentId">The identifier of the comment that was decided upon.</param>
    /// <param name="moderatorId">The identifier of the deciding moderator.</param>
    /// <param name="decision">The recorded moderation outcome.</param>
    /// <param name="decidedAtUtc">The UTC instant at which the decision was made.</param>
    /// <param name="reason">An optional explanation; when provided, length must not exceed 500.</param>
    public ModerationDecision(Guid commentId, Guid moderatorId, ModerationOutcome decision, DateTime decidedAtUtc, string? reason = null)
    {
        ValidateReason(reason);

        Id = Guid.NewGuid();
        CommentId = commentId;
        ModeratorId = moderatorId;
        Decision = decision;
        DecidedAtUtc = decidedAtUtc;
        Reason = reason;
    }

    /// <summary>
    /// Gets the primary key of the decision.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the comment that was decided upon.
    /// </summary>
    public Guid CommentId { get; private set; }

    /// <summary>
    /// Gets the identifier of the deciding moderator.
    /// </summary>
    public Guid ModeratorId { get; private set; }

    /// <summary>
    /// Gets the recorded moderation outcome.
    /// </summary>
    public ModerationOutcome Decision { get; private set; }

    /// <summary>
    /// Gets the UTC instant at which the decision was made.
    /// </summary>
    public DateTime DecidedAtUtc { get; private set; }

    /// <summary>
    /// Gets the optional explanation provided by the moderator.
    /// </summary>
    public string? Reason { get; private set; }

    private static void ValidateReason(string? reason)
    {
        if (reason is not null && reason.Length > MaxReasonLength)
        {
            throw new ArgumentException($"Reason must not exceed {MaxReasonLength} characters.", nameof(reason));
        }
    }
}
