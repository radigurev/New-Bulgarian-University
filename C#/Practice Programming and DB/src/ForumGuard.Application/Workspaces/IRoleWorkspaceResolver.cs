using System.Security.Claims;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Resolves the appropriate <see cref="IRoleWorkspace"/> Strategy for a signed-in principal (SDD-FORUM-011).
/// <para>Resolution precedence is Administrator &gt; Moderator &gt; User; an anonymous or null principal yields the User baseline.</para>
/// </summary>
public interface IRoleWorkspaceResolver
{
    /// <summary>
    /// Returns the workspace for the supplied principal based on its role claims.
    /// </summary>
    /// <param name="principal">The signed-in principal; <c>null</c> or anonymous resolves to the User workspace.</param>
    /// <returns>The matching role workspace; never <c>null</c>.</returns>
    IRoleWorkspace Resolve(ClaimsPrincipal? principal);
}
