namespace ForumGuard.Application.Workspaces;

/// <summary>
/// Represents an action a role's workspace may advertise (SDD-FORUM-011).
/// <para>The set advertised by each workspace must be consistent with the authorization policies (B-20 / V-5).</para>
/// </summary>
public enum WorkspaceAction
{
    /// <summary>
    /// Create a new forum thread (authoring action available to every user).
    /// </summary>
    CreateThread,

    /// <summary>
    /// Write a comment on a thread (authoring action available to every user).
    /// </summary>
    WriteComment,

    /// <summary>
    /// View the moderation queue of flagged comments (gated by CanModerateComments).
    /// </summary>
    ViewModerationQueue,

    /// <summary>
    /// Approve a flagged comment (gated by CanModerateComments).
    /// </summary>
    ApproveComment,

    /// <summary>
    /// Reject a flagged comment (gated by CanModerateComments).
    /// </summary>
    RejectComment,

    /// <summary>
    /// Grant or revoke the moderator role (gated by CanManageModerators).
    /// </summary>
    ManageModerators,

    /// <summary>
    /// Activate or deactivate user accounts (gated by CanManageAccounts).
    /// </summary>
    ManageAccounts
}
