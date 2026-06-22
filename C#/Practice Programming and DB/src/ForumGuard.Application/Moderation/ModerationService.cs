using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation.Dtos;
using ForumGuard.Application.Moderation.Interfaces;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Exceptions;
using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Application.Moderation;

/// <summary>
/// Orchestrates the moderator review workflow (SDD-FORUM-002): list the queue and apply Approve/Reject atomically.
/// <para>See <see cref="ICommentRepository"/>, <see cref="IModerationDecisionRepository"/>, and <see cref="IUnitOfWork"/>.</para>
/// </summary>
public sealed class ModerationService : IModerationService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IModerationDecisionRepository _decisionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes the moderation service with its persistence and clock dependencies.
    /// </summary>
    /// <param name="commentRepository">Repository used to load the queue and the target comment.</param>
    /// <param name="decisionRepository">Repository used to record the decision audit row.</param>
    /// <param name="unitOfWork">The commit boundary that persists the transition and audit row atomically.</param>
    /// <param name="clock">The UTC time source for decision and publication timestamps.</param>
    public ModerationService(
        ICommentRepository commentRepository,
        IModerationDecisionRepository decisionRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        ArgumentNullException.ThrowIfNull(commentRepository);
        ArgumentNullException.ThrowIfNull(decisionRepository);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(clock);
        _commentRepository = commentRepository;
        _decisionRepository = decisionRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<QueuedCommentDto>> GetQueueAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Comment> flagged = await _commentRepository.GetFlaggedQueueAsync(cancellationToken).ConfigureAwait(false);

        return flagged
            .Select(comment => new QueuedCommentDto(
                comment.Id,
                comment.AuthorId,
                comment.Body,
                comment.CreatedAtUtc,
                comment.AnalysisLabel,
                comment.AnalysisScore))
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<Result<ModerationDecisionResult>> DecideAsync(ModerationDecisionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<ModerationDecisionResult>? validation = ValidateRequest(request);
        if (validation is not null)
        {
            return validation;
        }

        Comment? comment = await _commentRepository.GetByIdAsync(request.CommentId, cancellationToken).ConfigureAwait(false);
        if (comment is null)
        {
            return Result<ModerationDecisionResult>.NotFound("The comment no longer exists.");
        }

        if (comment.Status != CommentStatus.FlaggedForReview)
        {
            return Result<ModerationDecisionResult>.Conflict("This comment was already handled by another moderator.");
        }

        return await ApplyDecisionAsync(request, comment, cancellationToken).ConfigureAwait(false);
    }

    private static Result<ModerationDecisionResult>? ValidateRequest(ModerationDecisionRequest request)
    {
        if (!Enum.IsDefined(request.Decision))
        {
            return Result<ModerationDecisionResult>.Validation("The moderation decision is missing or invalid.");
        }

        if (request.Reason is not null && request.Reason.Length > ModerationDecision.MaxReasonLength)
        {
            return Result<ModerationDecisionResult>.Validation($"The reason must not exceed {ModerationDecision.MaxReasonLength} characters.");
        }

        return null;
    }

    private async Task<Result<ModerationDecisionResult>> ApplyDecisionAsync(
        ModerationDecisionRequest request,
        Comment comment,
        CancellationToken cancellationToken)
    {
        DateTime decidedAtUtc = _clock.UtcNow;

        try
        {
            Transition(comment, request.Decision, decidedAtUtc);
        }
        catch (InvalidCommentTransitionException)
        {
            return Result<ModerationDecisionResult>.Conflict("This comment was already handled by another moderator.");
        }

        ModerationDecision decision = new(request.CommentId, request.ModeratorId, request.Decision, decidedAtUtc, request.Reason);

        _commentRepository.Update(comment);
        _decisionRepository.Add(decision);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<ModerationDecisionResult>.Success(new ModerationDecisionResult(comment.Id, comment.Status, decision.Id));
    }

    private static void Transition(Comment comment, ModerationOutcome decision, DateTime decidedAtUtc)
    {
        if (decision == ModerationOutcome.Approved)
        {
            comment.Approve(decidedAtUtc);
            return;
        }

        comment.Reject();
    }
}
