using ForumGuard.Web.Services.Dtos;

namespace ForumGuard.Web.Services.Interfaces;

/// <summary>
/// Defines read-only forum projections for the Web pages (SDD-FORUM-001 context, SDD-FORUM-010 visibility).
/// <para>Lists threads newest-first and composes a thread detail view of publicly visible comments
/// (oldest first), enriching identifiers with author display names.</para>
/// </summary>
public interface IForumReadService
{
    /// <summary>
    /// Lists forum threads ordered by creation time, newest first.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The thread rows for the home listing.</returns>
    Task<IReadOnlyList<ThreadListItem>> ListThreadsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the detail view for a thread, including its publicly visible comments oldest first.
    /// </summary>
    /// <param name="threadId">The identifier of the thread.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The thread detail view, or <c>null</c> when the thread does not exist.</returns>
    Task<ThreadDetailView?> GetThreadDetailAsync(Guid threadId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the moderation queue enriched with author display names, oldest first.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The queue rows for moderator review.</returns>
    Task<IReadOnlyList<QueueRowView>> GetQueueRowsAsync(CancellationToken cancellationToken = default);
}
