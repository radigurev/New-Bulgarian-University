namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Read model for a thread detail page: the thread header plus its publicly visible comments
/// (SDD-FORUM-001 §2.1, SDD-FORUM-010 visibility invariant).
/// </summary>
/// <param name="ThreadId">The thread identifier.</param>
/// <param name="Title">The thread title.</param>
/// <param name="CreatedByDisplayName">The display name of the thread creator.</param>
/// <param name="CreatedAtUtc">The UTC instant at which the thread was created.</param>
/// <param name="Comments">The visible comments, ordered oldest first.</param>
public sealed record ThreadDetailView(
    Guid ThreadId,
    string Title,
    string CreatedByDisplayName,
    DateTime CreatedAtUtc,
    IReadOnlyList<ThreadCommentView> Comments);
