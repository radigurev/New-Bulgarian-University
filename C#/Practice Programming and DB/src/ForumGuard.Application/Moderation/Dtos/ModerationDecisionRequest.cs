using ForumGuard.Domain.Enums;

namespace ForumGuard.Application.Moderation.Dtos;

/// <summary>
/// Carries a moderator's Approve or Reject decision on a flagged comment (SDD-FORUM-002).
/// <para>See <see cref="ForumGuard.Application.Moderation.Interfaces.IModerationService"/>.</para>
/// </summary>
/// <param name="CommentId">The identifier of the comment being decided.</param>
/// <param name="ModeratorId">The identifier of the acting moderator, resolved from the request principal.</param>
/// <param name="Decision">The recorded moderation outcome: Approved or Rejected.</param>
/// <param name="Reason">An optional explanation of at most 500 characters.</param>
public sealed record ModerationDecisionRequest(
    Guid CommentId,
    Guid ModeratorId,
    ModerationOutcome Decision,
    string? Reason = null);
