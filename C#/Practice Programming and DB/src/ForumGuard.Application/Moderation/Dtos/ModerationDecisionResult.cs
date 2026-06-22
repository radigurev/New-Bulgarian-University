using ForumGuard.Domain.Enums;

namespace ForumGuard.Application.Moderation.Dtos;

/// <summary>
/// Conveys the outcome of a successful moderation decision (SDD-FORUM-002).
/// <para>See <see cref="ForumGuard.Application.Moderation.Interfaces.IModerationService"/>.</para>
/// </summary>
/// <param name="CommentId">The identifier of the decided comment.</param>
/// <param name="Status">The terminal status reached: ApprovedByModerator or RejectedByModerator.</param>
/// <param name="DecisionId">The identifier of the recorded moderation-decision audit row.</param>
public sealed record ModerationDecisionResult(Guid CommentId, CommentStatus Status, Guid DecisionId);
