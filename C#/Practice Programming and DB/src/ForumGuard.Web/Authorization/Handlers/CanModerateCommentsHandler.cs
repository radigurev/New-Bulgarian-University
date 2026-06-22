using ForumGuard.Domain.Authorization;
using ForumGuard.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Authorization.Handlers;

/// <summary>
/// Evaluates <see cref="CanModerateCommentsRequirement"/> from role claims only (SDD-FORUM-011 B-4..B-7, B-14).
/// <para>Succeeds when the principal holds <see cref="ForumRoles.Moderator"/>; the
/// <see cref="ForumRoles.Administrator"/> role alone never satisfies it. A denial leaves the
/// requirement unmet and never throws (B-12).</para>
/// </summary>
public sealed class CanModerateCommentsHandler : AuthorizationHandler<CanModerateCommentsRequirement>
{
    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CanModerateCommentsRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (context.User.IsInRole(ForumRoles.Moderator))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
