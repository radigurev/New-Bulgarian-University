using ForumGuard.Domain.Entities;

namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines comment-specific data access on top of the generic repository contract.
/// <para>See <see cref="Comment"/>.</para>
/// </summary>
public interface ICommentRepository : IRepository<Comment>
{
    /// <summary>
    /// Returns the moderator queue: comments awaiting review, ordered oldest first.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The flagged comments in the moderator queue.</returns>
    Task<IReadOnlyList<Comment>> GetFlaggedQueueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the publicly visible comments for a thread, ordered by publication time.
    /// </summary>
    /// <param name="threadId">The identifier of the thread.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The visible comments for the thread.</returns>
    Task<IReadOnlyList<Comment>> GetPublishedByThreadAsync(Guid threadId, CancellationToken cancellationToken = default);
}
