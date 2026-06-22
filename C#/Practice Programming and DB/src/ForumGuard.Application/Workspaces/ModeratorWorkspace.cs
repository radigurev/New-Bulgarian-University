using ForumGuard.Domain.Authorization;

namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Workspace Strategy for the <c>Moderator</c> role: comment-moderation actions only (SDD-FORUM-011 B-17).
/// <para>Advertises the queue, Approve, and Reject; never moderator-management or account-management.</para>
/// </summary>
public sealed class ModeratorWorkspace : IRoleWorkspace
{
    private static readonly IReadOnlyList<WorkspaceMenuItem> Menu =
    [
        new WorkspaceMenuItem("Moderation queue", "/Moderation/Queue", WorkspaceAction.ViewModerationQueue),
        new WorkspaceMenuItem("Approve", "/Moderation/Queue", WorkspaceAction.ApproveComment),
        new WorkspaceMenuItem("Reject", "/Moderation/Queue", WorkspaceAction.RejectComment)
    ];

    private static readonly IReadOnlySet<WorkspaceAction> Actions = new HashSet<WorkspaceAction>
    {
        WorkspaceAction.ViewModerationQueue,
        WorkspaceAction.ApproveComment,
        WorkspaceAction.RejectComment
    };

    /// <inheritdoc />
    public string RoleName => ForumRoles.Moderator;

    /// <inheritdoc />
    public string DashboardTitle => "Moderation";

    /// <inheritdoc />
    public IReadOnlyList<WorkspaceMenuItem> MenuItems => Menu;

    /// <inheritdoc />
    public IReadOnlySet<WorkspaceAction> AvailableActions => Actions;
}
