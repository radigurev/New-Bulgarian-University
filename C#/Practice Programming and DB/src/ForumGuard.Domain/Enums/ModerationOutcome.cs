namespace ForumGuard.Domain.Enums;

/// <summary>
/// Represents the decision a moderator records for a flagged comment.
/// </summary>
public enum ModerationOutcome
{
    /// <summary>
    /// The moderator approved the comment for public visibility.
    /// </summary>
    Approved,

    /// <summary>
    /// The moderator rejected the comment, keeping it hidden.
    /// </summary>
    Rejected
}
