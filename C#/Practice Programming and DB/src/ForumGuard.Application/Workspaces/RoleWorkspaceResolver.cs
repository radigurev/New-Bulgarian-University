using System.Security.Claims;
using ForumGuard.Domain.Authorization;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Resolves a role workspace by role-claim precedence Administrator &gt; Moderator &gt; User (SDD-FORUM-011 B-19).
/// <para>Depends on the <see cref="IRoleWorkspace"/> abstraction only; treats a null, anonymous, or
/// unrecognized principal as the User baseline and never throws (B-21).</para>
/// </summary>
public sealed class RoleWorkspaceResolver : IRoleWorkspaceResolver
{
    private readonly IReadOnlyDictionary<string, IRoleWorkspace> _workspacesByRole;
    private readonly IRoleWorkspace _userWorkspace;

    /// <summary>
    /// Initializes the resolver from the registered <see cref="IRoleWorkspace"/> Strategies.
    /// </summary>
    /// <param name="workspaces">The role workspace Strategies, keyed internally on <see cref="IRoleWorkspace.RoleName"/>.</param>
    public RoleWorkspaceResolver(IEnumerable<IRoleWorkspace> workspaces)
    {
        ArgumentNullException.ThrowIfNull(workspaces);

        Dictionary<string, IRoleWorkspace> lookup = new(StringComparer.Ordinal);
        foreach (IRoleWorkspace workspace in workspaces)
        {
            lookup[workspace.RoleName] = workspace;
        }

        _workspacesByRole = lookup;

        if (!lookup.TryGetValue(ForumRoles.User, out IRoleWorkspace? userWorkspace))
        {
            throw new InvalidOperationException($"No {nameof(IRoleWorkspace)} is registered for role '{ForumRoles.User}'.");
        }

        _userWorkspace = userWorkspace;
    }

    /// <inheritdoc />
    public IRoleWorkspace Resolve(ClaimsPrincipal? principal)
    {
        if (principal?.Identity is null || !principal.Identity.IsAuthenticated)
        {
            return _userWorkspace;
        }

        if (principal.IsInRole(ForumRoles.Administrator) && _workspacesByRole.TryGetValue(ForumRoles.Administrator, out IRoleWorkspace? administratorWorkspace))
        {
            return administratorWorkspace;
        }

        if (principal.IsInRole(ForumRoles.Moderator) && _workspacesByRole.TryGetValue(ForumRoles.Moderator, out IRoleWorkspace? moderatorWorkspace))
        {
            return moderatorWorkspace;
        }

        return _userWorkspace;
    }
}
