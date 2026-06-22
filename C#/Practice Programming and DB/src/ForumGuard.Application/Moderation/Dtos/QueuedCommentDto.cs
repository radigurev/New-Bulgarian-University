using ForumGuard.Domain.Enums;

namespace ForumGuard.Application.Moderation.Dtos;

/// <summary>
/// Read model for one entry in the moderator review queue (SDD-FORUM-002 B2); never exposes an ORM entity.
/// <para>See <see cref="ForumGuard.Application.Moderation.Interfaces.IModerationService"/>.</para>
/// </summary>
/// <param name="CommentId">The identifier of the flagged comment.</param>
/// <param name="AuthorId">The identifier of the comment's author.</param>
/// <param name="Body">The comment text under review.</param>
/// <param name="CreatedAtUtc">The UTC instant at which the comment was created.</param>
/// <param name="AnalysisLabel">The toxicity label assigned by analysis, if any.</param>
/// <param name="AnalysisScore">The toxic-score assigned by analysis, if any.</param>
public sealed record QueuedCommentDto(
    Guid CommentId,
    Guid AuthorId,
    string Body,
    DateTime CreatedAtUtc,
    ToxicityLabel? AnalysisLabel,
    float? AnalysisScore);
