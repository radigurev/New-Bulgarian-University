using ForumGuard.Domain.Authorization;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Workspace Strategy for the <c>User</c> role: authoring actions only (SDD-FORUM-011 B-16).
/// <para>Advertises thread creation and comment writing; never moderation or management actions.</para>
/// </summary>
public sealed class UserWorkspace : IRoleWorkspace
{
    private static readonly IReadOnlyList<WorkspaceMenuItem> Menu =
    [
        new WorkspaceMenuItem("Threads", "/", WorkspaceAction.CreateThread),
        new WorkspaceMenuItem("Write a comment", "/", WorkspaceAction.WriteComment)
    ];

    private static readonly IReadOnlySet<WorkspaceAction> Actions = new HashSet<WorkspaceAction>
    {
        WorkspaceAction.CreateThread,
        WorkspaceAction.WriteComment
    };

    /// <inheritdoc />
    public string RoleName => ForumRoles.User;

    /// <inheritdoc />
    public string DashboardTitle => "My Forum";

    /// <inheritdoc />
    public IReadOnlyList<WorkspaceMenuItem> MenuItems => Menu;

    /// <inheritdoc />
    public IReadOnlySet<WorkspaceAction> AvailableActions => Actions;
}
