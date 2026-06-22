using ForumGuard.Domain.Enums;

namespace ForumGuard.Domain.Lifecycle;

/// <summary>
/// Provides the pure, side-effect-free transition guard for the comment lifecycle state machine.
/// <para>See <see cref="CommentStatus"/> for the set of states.</para>
/// </summary>
public static class CommentStatusTransitions
{
    /// <summary>
    /// Determines whether a transition from <paramref name="from"/> to <paramref name="to"/> is permitted.
    /// </summary>
    /// <param name="from">The current comment status.</param>
    /// <param name="to">The requested target status.</param>
    /// <returns><c>true</c> for the four runtime transitions defined in SDD-FORUM-010 §3.3; otherwise <c>false</c> — including self-transitions and any move into <see cref="CommentStatus.PendingAnalysis"/>. Initial construction into <see cref="CommentStatus.PendingAnalysis"/> is set by the comment constructor, not this guard.</returns>
    public static bool IsAllowed(CommentStatus from, CommentStatus to)
    {
        return (from, to) switch
        {
            (CommentStatus.PendingAnalysis, CommentStatus.Published) => true,
            (CommentStatus.PendingAnalysis, CommentStatus.FlaggedForReview) => true,
            (CommentStatus.FlaggedForReview, CommentStatus.ApprovedByModerator) => true,
            (CommentStatus.FlaggedForReview, CommentStatus.RejectedByModerator) => true,
            _ => false
        };
    }
}
