using ForumGuard.Application.Common;
using ForumGuard.Domain.Authorization;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ForumGuard.Web.Services;

/// <summary>
/// Implements Administrator grant/revoke of the Moderator role over ASP.NET Core Identity (SDD-FORUM-004).
/// <para>Grant requires an active target; redundant grant/revoke is an idempotent no-op; the
/// Administrator role is never mutated by this flow.</para>
/// </summary>
public sealed class ModeratorRoleService : IModeratorRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ModeratorRoleService> _logger;

    /// <summary>
    /// Initializes the service with the Identity user manager and logger.
    /// </summary>
    /// <param name="userManager">The Identity user store manager.</param>
    /// <param name="logger">The logger for role-change outcomes.</param>
    public ModeratorRoleService(UserManager<ApplicationUser> userManager, ILogger<ModeratorRoleService> logger)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(logger);
        _userManager = userManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<ModeratorRoleChangeOutcome>> GrantModeratorAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUser? user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result<ModeratorRoleChangeOutcome>.NotFound("User not found.");
        }

        if (!user.IsActive)
        {
            return Result<ModeratorRoleChangeOutcome>.Validation("Cannot grant Moderator to an inactive account.");
        }

        if (await _userManager.IsInRoleAsync(user, ForumRoles.Moderator).ConfigureAwait(false))
        {
            return Result<ModeratorRoleChangeOutcome>.Success(ModeratorRoleChangeOutcome.NoOp);
        }

        IdentityResult result = await _userManager.AddToRoleAsync(user, ForumRoles.Moderator).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return Result<ModeratorRoleChangeOutcome>.Conflict("Could not update the Moderator role. Please try again.");
        }

        _logger.LogInformation("Granted Moderator role to {UserId}.", user.Id);
        return Result<ModeratorRoleChangeOutcome>.Success(ModeratorRoleChangeOutcome.Granted);
    }

    /// <inheritdoc />
    public async Task<Result<ModeratorRoleChangeOutcome>> RevokeModeratorAsync(Guid targetUserId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUser? user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result<ModeratorRoleChangeOutcome>.NotFound("User not found.");
        }

        if (!await _userManager.IsInRoleAsync(user, ForumRoles.Moderator).ConfigureAwait(false))
        {
            return Result<ModeratorRoleChangeOutcome>.Success(ModeratorRoleChangeOutcome.NoOp);
        }

        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, ForumRoles.Moderator).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return Result<ModeratorRoleChangeOutcome>.Conflict("Could not update the Moderator role. Please try again.");
        }

        _logger.LogInformation("Revoked Moderator role from {UserId}.", user.Id);
        return Result<ModeratorRoleChangeOutcome>.Success(ModeratorRoleChangeOutcome.Revoked);
    }
}
