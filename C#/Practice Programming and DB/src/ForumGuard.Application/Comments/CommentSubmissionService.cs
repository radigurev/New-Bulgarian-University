using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Comments.Interfaces;
using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation;
using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ForumGuard.Application.Comments;

/// <summary>
/// Orchestrates comment submission (SDD-FORUM-001): gate, validate, create in PendingAnalysis, analyze, and persist.
/// <para>See <see cref="ICommentModerationPipeline"/>, <see cref="ICommentRepository"/>, and <see cref="IUnitOfWork"/>.</para>
/// </summary>
public sealed class CommentSubmissionService : ICommentSubmissionService
{
    private readonly IForumThreadRepository _threadRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ICommentModerationPipeline _pipeline;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly IOptionsMonitor<ModerationOptions> _options;
    private readonly ILogger<CommentSubmissionService> _logger;

    /// <summary>
    /// Initializes the submission service with its persistence, pipeline, clock, options, and logging dependencies.
    /// </summary>
    /// <param name="threadRepository">Repository used to confirm the target thread exists.</param>
    /// <param name="commentRepository">Repository used to track the new comment.</param>
    /// <param name="pipeline">The moderation pipeline that classifies the comment.</param>
    /// <param name="unitOfWork">The commit boundary persisting the comment.</param>
    /// <param name="clock">The UTC time source for timestamps.</param>
    /// <param name="options">The monitor supplying the effective maximum comment length.</param>
    /// <param name="logger">The logger used to record fail-safe analysis errors.</param>
    public CommentSubmissionService(
        IForumThreadRepository threadRepository,
        ICommentRepository commentRepository,
        ICommentModerationPipeline pipeline,
        IUnitOfWork unitOfWork,
        IClock clock,
        IOptionsMonitor<ModerationOptions> options,
        ILogger<CommentSubmissionService> logger)
    {
        ArgumentNullException.ThrowIfNull(threadRepository);
        ArgumentNullException.ThrowIfNull(commentRepository);
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);
        _threadRepository = threadRepository;
        _commentRepository = commentRepository;
        _pipeline = pipeline;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _options = options;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<SubmitCommentResult>> SubmitCommentAsync(SubmitCommentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<string> gate = ValidateSubmission(request);
        if (!gate.IsSuccess)
        {
            return Demote(gate);
        }

        ForumThread? thread = await _threadRepository.GetByIdAsync(request.ThreadId, cancellationToken).ConfigureAwait(false);
        if (thread is null)
        {
            return Result<SubmitCommentResult>.NotFound("The target thread does not exist.");
        }

        return await CreateAnalyzeAndPersistAsync(request, gate.Value!, cancellationToken).ConfigureAwait(false);
    }

    private Result<string> ValidateSubmission(SubmitCommentRequest request)
    {
        if (request.AuthorId != request.SignedInUserId)
        {
            return Result<string>.Forbidden("A comment may only be authored by the signed-in user.");
        }

        if (!request.IsAuthorActive)
        {
            return Result<string>.Forbidden("The account is deactivated; contact an administrator.");
        }

        return ValidateBody(request.Body);
    }

    private Result<string> ValidateBody(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return Result<string>.Validation("The comment body is required.");
        }

        string trimmed = body.Trim();
        int maxLength = Math.Min(_options.CurrentValue.MaxCommentLength, Comment.MaxBodyLength);

        return trimmed.Length > maxLength
            ? Result<string>.Validation($"The comment body must not exceed {maxLength} characters.")
            : Result<string>.Success(trimmed);
    }

    private async Task<Result<SubmitCommentResult>> CreateAnalyzeAndPersistAsync(
        SubmitCommentRequest request,
        string trimmedBody,
        CancellationToken cancellationToken)
    {
        DateTime createdAtUtc = _clock.UtcNow;
        Comment comment = new(request.ThreadId, request.AuthorId, trimmedBody, createdAtUtc);

        ApplyAnalysisOutcome(comment, await ResolveVerdictAsync(comment, cancellationToken).ConfigureAwait(false));

        _commentRepository.Add(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result<SubmitCommentResult>.Success(new SubmitCommentResult(comment.Id, comment.Status));
    }

    private async Task<AnalysisResult?> ResolveVerdictAsync(Comment comment, CancellationToken cancellationToken)
    {
        try
        {
            ModerationVerdict verdict = await _pipeline
                .EvaluateAsync(new CommentModerationContext(comment.Body, comment.Id), cancellationToken)
                .ConfigureAwait(false);

            return new AnalysisResult(verdict.Label, verdict.Score);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Moderation analysis failed for comment {CommentId}; flagging for review.", comment.Id);
            return null;
        }
    }

    private void ApplyAnalysisOutcome(Comment comment, AnalysisResult? result)
    {
        if (result is null)
        {
            comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 1.0f), _clock.UtcNow);
            return;
        }

        comment.ApplyAnalysis(result, _clock.UtcNow);
    }

    private static Result<SubmitCommentResult> Demote(Result<string> failure) => failure.Status switch
    {
        ResultStatus.Validation => Result<SubmitCommentResult>.Validation(failure.Error!),
        ResultStatus.Forbidden => Result<SubmitCommentResult>.Forbidden(failure.Error!),
        ResultStatus.NotFound => Result<SubmitCommentResult>.NotFound(failure.Error!),
        ResultStatus.Conflict => Result<SubmitCommentResult>.Conflict(failure.Error!),
        _ => Result<SubmitCommentResult>.Validation(failure.Error ?? "Invalid submission.")
    };
}
