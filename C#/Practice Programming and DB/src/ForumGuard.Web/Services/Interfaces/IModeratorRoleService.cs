using ForumGuard.Application.Common;
using ForumGuard.Web.Services.Dtos;

namespace ForumGuard.Web.Services.Interfaces;

/// <summary>
/// Defines the Administrator use cases for granting and revoking the Moderator role (SDD-FORUM-004).
/// <para>Grant requires an active target; revoke is permitted regardless of active state. Redundant
/// grant/revoke is an idempotent no-op. The Administrator role is never mutated here.</para>
/// </summary>
public interface IModeratorRoleService
{
    /// <summary>
    /// Grants the Moderator role to an active target account.
    /// </summary>
    /// <param name="targetUserId">The identifier of the target account.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result describing the outcome, or a typed failure (not-found/validation/conflict).</returns>
    Task<Result<ModeratorRoleChangeOutcome>> GrantModeratorAsync(Guid targetUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes the Moderator role from a target account.
    /// </summary>
    /// <param name="targetUserId">The identifier of the target account.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result describing the outcome, or a typed failure (not-found/conflict).</returns>
    Task<Result<ModeratorRoleChangeOutcome>> RevokeModeratorAsync(Guid targetUserId, CancellationToken cancellationToken = default);
}
