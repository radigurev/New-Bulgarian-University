using ForumGuard.Domain.Authorization;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Defines the Strategy contract that composes a single role's dashboard, menu, and available actions (SDD-FORUM-011).
/// <para>Concrete strategies are selected by <see cref="IRoleWorkspaceResolver"/>; the advertised
/// <see cref="AvailableActions"/> must stay consistent with the role's authorization policies.</para>
/// </summary>
public interface IRoleWorkspace
{
    /// <summary>
    /// Gets the role this workspace serves; one of the values in <see cref="ForumRoles"/>.
    /// </summary>
    string RoleName { get; }

    /// <summary>
    /// Gets the heading shown at the top of the role's dashboard.
    /// </summary>
    string DashboardTitle { get; }

    /// <summary>
    /// Gets the ordered navigation menu items presented to the role.
    /// </summary>
    IReadOnlyList<WorkspaceMenuItem> MenuItems { get; }

    /// <summary>
    /// Gets the set of actions the role is permitted to perform from this workspace.
    /// </summary>
    IReadOnlySet<WorkspaceAction> AvailableActions { get; }
}
