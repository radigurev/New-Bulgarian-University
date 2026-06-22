using ForumGuard.Domain.Entities;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Selects the forum threads created by a specific user, ordered by
/// <see cref="ForumThread.CreatedAtUtc"/> descending (newest first).
/// <para>See SDD-FORUM-022 (B-17). Backs <c>IForumThreadRepository.GetByCreatorAsync</c>.</para>
/// </summary>
public sealed class ForumThreadsByCreatorSpecification : Specification<ForumThread>
{
    /// <summary>
    /// Initializes the specification scoped to a creating user.
    /// </summary>
    /// <param name="createdById">The identifier of the creating user.</param>
    public ForumThreadsByCreatorSpecification(Guid createdById)
        : base(thread => thread.CreatedById == createdById)
    {
        ApplyOrderByDescending(thread => thread.CreatedAtUtc);
    }
}
