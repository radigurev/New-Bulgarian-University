using ForumGuard.Domain.Entities;

namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines forum-thread-specific data access on top of the generic repository contract.
/// <para>See <see cref="ForumThread"/>.</para>
/// </summary>
public interface IForumThreadRepository : IRepository<ForumThread>
{
    /// <summary>
    /// Returns all forum threads created by the specified user.
    /// </summary>
    /// <param name="createdById">The identifier of the creating user.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The threads created by the user.</returns>
    Task<IReadOnlyList<ForumThread>> GetByCreatorAsync(Guid createdById, CancellationToken cancellationToken = default);
}
