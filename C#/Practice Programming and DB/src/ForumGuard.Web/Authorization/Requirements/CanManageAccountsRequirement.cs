using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Authorization.Requirements;

/// <summary>
/// The authorization requirement gating account activation management (SDD-FORUM-011 B-10, B-11).
/// <para>Satisfied by the Administrator role only. Evaluated by
/// <see cref="Handlers.CanManageAccountsHandler"/>.</para>
/// </summary>
public sealed class CanManageAccountsRequirement : IAuthorizationRequirement
{
}
