namespace ForumGuard.Domain.Enums;

/// <summary>
/// Represents the lifecycle state of a comment in the moderation state machine.
/// </summary>
public enum CommentStatus
{
    /// <summary>
    /// The comment has been created and is awaiting automatic analysis.
    /// </summary>
    PendingAnalysis,

    /// <summary>
    /// The comment was classified as clean and is publicly visible. Terminal state.
    /// </summary>
    Published,

    /// <summary>
    /// The comment was classified as toxic and awaits moderator review. Hidden.
    /// </summary>
    FlaggedForReview,

    /// <summary>
    /// A moderator approved the flagged comment, making it publicly visible. Terminal state.
    /// </summary>
    ApprovedByModerator,

    /// <summary>
    /// A moderator rejected the flagged comment, keeping it hidden. Terminal state.
    /// </summary>
    RejectedByModerator
}
