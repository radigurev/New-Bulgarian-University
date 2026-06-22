namespace ForumGuard.Web.Authorization;

/// <summary>
/// Provides the stable, distinct authorization policy-name constants for ForumGuard (SDD-FORUM-011 B-13, V-2).
/// <para>Each name binds exactly one requirement: <see cref="Requirements.CanModerateCommentsRequirement"/>,
/// <see cref="Requirements.CanManageModeratorsRequirement"/>, or
/// <see cref="Requirements.CanManageAccountsRequirement"/>.</para>
/// </summary>
public static class ForumPolicies
{
    /// <summary>
    /// The policy permitting comment moderation; satisfied only by the Moderator role, never by Administrator alone.
    /// </summary>
    public const string CanModerateComments = "CanModerateComments";

    /// <summary>
    /// The policy permitting moderator-role management; satisfied only by the Administrator role.
    /// </summary>
    public const string CanManageModerators = "CanManageModerators";

    /// <summary>
    /// The policy permitting account activation management; satisfied only by the Administrator role.
    /// </summary>
    public const string CanManageAccounts = "CanManageAccounts";
}
