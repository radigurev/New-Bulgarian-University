using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Persistence.Specifications;

namespace ForumGuard.Infrastructure.Persistence.Repositories;

/// <summary>
/// Provides moderation-decision-specific data access on top of the generic repository.
/// <para>See SDD-FORUM-022 (B-17).</para>
/// </summary>
public sealed class ModerationDecisionRepository : Repository<ModerationDecision>, IModerationDecisionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModerationDecisionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ModerationDecisionRepository(ForumGuardDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ModerationDecision>> GetByCommentIdAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        return await ListAsync(new ModerationDecisionsByCommentSpecification(commentId), cancellationToken).ConfigureAwait(false);
    }
}
