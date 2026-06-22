using ForumGuard.Domain.Authorization;
using ForumGuard.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Authorization.Handlers;

/// <summary>
/// Evaluates <see cref="CanManageAccountsRequirement"/> from role claims only (SDD-FORUM-011 B-10, B-11, B-14).
/// <para>Succeeds when the principal holds <see cref="ForumRoles.Administrator"/>; a Moderator-only or
/// User-only principal is denied. A denial leaves the requirement unmet and never throws (B-12).</para>
/// </summary>
public sealed class CanManageAccountsHandler : AuthorizationHandler<CanManageAccountsRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanManageAccountsRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (context.User.IsInRole(ForumRoles.Administrator))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
