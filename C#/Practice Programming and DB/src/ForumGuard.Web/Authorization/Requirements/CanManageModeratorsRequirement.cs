using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Authorization.Requirements;

/// <summary>
/// The authorization requirement gating moderator-role management (SDD-FORUM-011 B-8, B-9).
/// <para>Satisfied by the Administrator role only. Evaluated by
/// <see cref="Handlers.CanManageModeratorsHandler"/>.</para>
/// </summary>
public sealed class CanManageModeratorsRequirement : IAuthorizationRequirement
{
}
