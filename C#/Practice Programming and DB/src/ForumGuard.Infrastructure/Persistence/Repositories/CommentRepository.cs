using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Persistence.Specifications;

namespace ForumGuard.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides comment-specific data access on top of the generic repository.
/// <para>See SDD-FORUM-022 (B-17). Intent-revealing queries are expressed through the relevant
/// specifications rather than ad-hoc inline LINQ.</para>
/// </summary>
public sealed class CommentRepository : Repository<Comment>, ICommentRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CommentRepository(ForumGuardDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Comment>> GetFlaggedQueueAsync(CancellationToken cancellationToken = default)
    {
        return await ListAsync(new FlaggedPendingReviewSpecification(), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Comment>> GetPublishedByThreadAsync(Guid threadId, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new PublishedCommentsByThreadSpecification(threadId), cancellationToken).ConfigureAwait(false);
    }
}
