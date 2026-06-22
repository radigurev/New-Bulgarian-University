namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Read model for a single publicly visible comment shown on a thread detail page (SDD-FORUM-001).
/// </summary>
/// <param name="CommentId">The comment identifier.</param>
/// <param name="AuthorDisplayName">The display name of the comment's author.</param>
/// <param name="Body">The comment text.</param>
/// <param name="PublishedAtUtc">The UTC instant at which the comment became publicly visible.</param>
public sealed record ThreadCommentView(
    Guid CommentId,
    string AuthorDisplayName,
    string Body,
    DateTime? PublishedAtUtc);
