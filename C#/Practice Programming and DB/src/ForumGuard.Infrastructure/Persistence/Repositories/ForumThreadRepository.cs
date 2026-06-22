using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Persistence.Specifications;

namespace ForumGuard.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides forum-thread-specific data access on top of the generic repository.
/// <para>See SDD-FORUM-022 (B-17).</para>
/// </summary>
public sealed class ForumThreadRepository : Repository<ForumThread>, IForumThreadRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumThreadRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ForumThreadRepository(ForumGuardDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ForumThread>> GetByCreatorAsync(Guid createdById, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new ForumThreadsByCreatorSpecification(createdById), cancellationToken).ConfigureAwait(false);
    }
}
