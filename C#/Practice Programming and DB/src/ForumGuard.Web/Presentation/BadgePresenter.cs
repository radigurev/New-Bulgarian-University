using ForumGuard.Domain.Authorization;
using ForumGuard.Domain.Enums;

namespace ForumGuard.Web.Presentation;

/// <summary>
/// Maps domain roles and comment statuses to their presentation labels and CSS modifier classes.
/// <para>Keeps presentation mapping out of the views and page handlers; used by the layout and pages.</para>
/// </summary>
public static class BadgePresenter
{
    /// <summary>
    /// Returns the short role badge label (USER / MOD / ADMIN) for the most privileged role.
    /// </summary>
    /// <param name="isAdministrator">Whether the principal holds the Administrator role.</param>
    /// <param name="isModerator">Whether the principal holds the Moderator role.</param>
    /// <returns>The badge label.</returns>
    public static string RoleLabel(bool isAdministrator, bool isModerator)
    {
        if (isAdministrator)
        {
            return "ADMIN";
        }

        return isModerator ? "MOD" : "USER";
    }

    /// <summary>
    /// Returns the role badge CSS modifier class for the most privileged role.
    /// </summary>
    /// <param name="isAdministrator">Whether the principal holds the Administrator role.</param>
    /// <param name="isModerator">Whether the principal holds the Moderator role.</param>
    /// <returns>The CSS modifier class.</returns>
    public static string RoleClass(bool isAdministrator, bool isModerator)
    {
        if (isAdministrator)
        {
            return "role-admin";
        }

        return isModerator ? "role-mod" : "role-user";
    }

    /// <summary>
    /// Returns the role badge label for a single Identity role name.
    /// </summary>
    /// <param name="roleName">One of the <see cref="ForumRoles"/> constants.</param>
    /// <returns>The badge label.</returns>
    public static string RoleLabelFor(string roleName) => roleName switch
    {
        ForumRoles.Administrator => "ADMIN",
        ForumRoles.Moderator => "MOD",
        _ => "USER"
    };

    /// <summary>
    /// Returns the role badge CSS class for a single Identity role name.
    /// </summary>
    /// <param name="roleName">One of the <see cref="ForumRoles"/> constants.</param>
    /// <returns>The CSS modifier class.</returns>
    public static string RoleClassFor(string roleName) => roleName switch
    {
        ForumRoles.Administrator => "role-admin",
        ForumRoles.Moderator => "role-mod",
        _ => "role-user"
    };

    /// <summary>
    /// Returns the human-readable status label shown on a status badge.
    /// </summary>
    /// <param name="status">The comment lifecycle status.</param>
    /// <returns>The status label.</returns>
    public static string StatusLabel(CommentStatus status) => status switch
    {
        CommentStatus.Published => "PUBLISHED",
        CommentStatus.ApprovedByModerator => "PUBLISHED",
        CommentStatus.FlaggedForReview => "FLAGGED",
        CommentStatus.RejectedByModerator => "REJECTED",
        _ => "AWAITING REVIEW"
    };

    /// <summary>
    /// Returns the status badge CSS modifier class for a comment status.
    /// </summary>
    /// <param name="status">The comment lifecycle status.</param>
    /// <returns>The CSS modifier class.</returns>
    public static string StatusClass(CommentStatus status) => status switch
    {
        CommentStatus.Published => "status-published",
        CommentStatus.ApprovedByModerator => "status-published",
        CommentStatus.FlaggedForReview => "status-flagged",
        CommentStatus.RejectedByModerator => "status-rejected",
        _ => "status-awaiting"
    };
}
