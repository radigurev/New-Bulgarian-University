using ForumGuard.Domain.Authorization;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Workspace Strategy for the <c>Administrator</c> role: management actions only (SDD-FORUM-011 B-18).
/// <para>Advertises manage-moderators and manage-accounts; never any comment-moderation action (consistent with B-5).</para>
/// </summary>
public sealed class AdministratorWorkspace : IRoleWorkspace
{
    private static readonly IReadOnlyList<WorkspaceMenuItem> Menu =
    [
        new WorkspaceMenuItem("Manage moderators", "/Admin/Moderators", WorkspaceAction.ManageModerators),
        new WorkspaceMenuItem("Manage accounts", "/Admin/Users", WorkspaceAction.ManageAccounts)
    ];

    private static readonly IReadOnlySet<WorkspaceAction> Actions = new HashSet<WorkspaceAction>
    {
        WorkspaceAction.ManageModerators,
        WorkspaceAction.ManageAccounts
    };

    /// <inheritdoc />
    public string RoleName => ForumRoles.Administrator;

    /// <inheritdoc />
    public string DashboardTitle => "Administration";

    /// <inheritdoc />
    public IReadOnlyList<WorkspaceMenuItem> MenuItems => Menu;

    /// <inheritdoc />
    public IReadOnlySet<WorkspaceAction> AvailableActions => Actions;
}
