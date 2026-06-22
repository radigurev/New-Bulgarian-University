using ForumGuard.Domain.Enums;

namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Read model for one row of the moderation queue page (SDD-FORUM-002 B2), enriching a
/// <c>QueuedCommentDto</c> with the author's display name for presentation.
/// </summary>
/// <param name="CommentId">The flagged comment identifier.</param>
/// <param name="AuthorDisplayName">The display name of the comment's author.</param>
/// <param name="Body">The comment text under review.</param>
/// <param name="CreatedAtUtc">The UTC instant at which the comment was created.</param>
/// <param name="AnalysisLabel">The toxicity label assigned by analysis, if any.</param>
/// <param name="AnalysisScore">The toxic-score assigned by analysis, if any.</param>
public sealed record QueueRowView(
    Guid CommentId,
    string AuthorDisplayName,
    string Body,
    DateTime CreatedAtUtc,
    ToxicityLabel? AnalysisLabel,
    float? AnalysisScore);
