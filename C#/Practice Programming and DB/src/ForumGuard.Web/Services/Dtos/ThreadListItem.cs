namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Read model for one row of the thread listing on the home page (SDD-FORUM-001 context).
/// </summary>
/// <param name="ThreadId">The thread identifier.</param>
/// <param name="Title">The thread title.</param>
/// <param name="CreatedByDisplayName">The display name of the user who created the thread.</param>
/// <param name="CreatedAtUtc">The UTC instant at which the thread was created.</param>
/// <param name="VisibleCommentCount">The count of publicly visible comments on the thread.</param>
public sealed record ThreadListItem(
    Guid ThreadId,
    string Title,
    string CreatedByDisplayName,
    DateTime CreatedAtUtc,
    int VisibleCommentCount);
