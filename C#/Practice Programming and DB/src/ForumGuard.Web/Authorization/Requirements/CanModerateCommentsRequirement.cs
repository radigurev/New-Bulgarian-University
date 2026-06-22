using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Authorization.Requirements;

/// <summary>
/// The authorization requirement gating comment moderation (SDD-FORUM-011 B-4..B-7).
/// <para>Satisfied by the Moderator role only; the Administrator role alone is excluded. Evaluated by
/// <see cref="Handlers.CanModerateCommentsHandler"/>.</para>
/// </summary>
public sealed class CanModerateCommentsRequirement : IAuthorizationRequirement
{
}
