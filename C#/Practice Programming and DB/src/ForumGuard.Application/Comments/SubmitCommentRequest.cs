namespace ForumGuard.Application.Comments;

/// <summary>
/// Carries the input required to submit a comment for analysis and publishing (SDD-FORUM-001).
/// <para>See <see cref="Interfaces.ICommentSubmissionService"/> and <see cref="SubmitCommentResult"/>.</para>
/// </summary>
/// <param name="ThreadId">The identifier of the target forum thread.</param>
/// <param name="AuthorId">The identifier of the authoring user; must equal the signed-in user.</param>
/// <param name="SignedInUserId">The identifier of the signed-in user resolved from the request principal.</param>
/// <param name="Body">The comment text to submit.</param>
/// <param name="IsAuthorActive">Whether the signed-in account is active; an inactive account is rejected.</param>
public sealed record SubmitCommentRequest(
    Guid ThreadId,
    Guid AuthorId,
    Guid SignedInUserId,
    string? Body,
    bool IsAuthorActive);
