using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation.Dtos;

namespace ForumGuard.Application.Moderation.Interfaces;

/// <summary>
/// Defines the moderator review use cases: list the queue and decide (Approve/Reject) a flagged comment (SDD-FORUM-002).
/// <para>See <see cref="QueuedCommentDto"/> and <see cref="ModerationDecisionRequest"/>.</para>
/// </summary>
public interface IModerationService
{
    /// <summary>
    /// Returns the queue of comments awaiting review, ordered oldest first.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The flagged comments awaiting a moderation decision.</returns>
    Task<IReadOnlyList<QueuedCommentDto>> GetQueueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a moderator's Approve or Reject decision atomically and records the audit row.
    /// </summary>
    /// <param name="request">The decision input.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result identifying the comment's terminal status, or a typed failure.</returns>
    Task<Result<ModerationDecisionResult>> DecideAsync(ModerationDecisionRequest request, CancellationToken cancellationToken = default);
}
