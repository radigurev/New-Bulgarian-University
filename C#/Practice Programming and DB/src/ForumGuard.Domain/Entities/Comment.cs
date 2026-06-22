using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Exceptions;
using ForumGuard.Domain.Lifecycle;

namespace ForumGuard.Domain.Entities;

/// <summary>
/// Represents a user comment on a forum thread and enforces its moderation lifecycle.
/// <para>See <see cref="CommentStatus"/>, <see cref="CommentStatusTransitions"/>, and <see cref="AnalysisResult"/>.</para>
/// </summary>
public sealed class Comment
{
    /// <summary>
    /// The maximum permitted length of <see cref="Body"/>.
    /// </summary>
    public const int MaxBodyLength = 4000;

    private Comment()
    {
        Body = string.Empty;
    }

    /// <summary>
    /// Creates a new comment in the <see cref="CommentStatus.PendingAnalysis"/> state.
    /// </summary>
    /// <param name="threadId">The identifier of the owning forum thread.</param>
    /// <param name="authorId">The identifier of the authoring user.</param>
    /// <param name="body">The comment text; required, non-whitespace, length 1..4000.</param>
    /// <param name="createdAtUtc">The UTC instant at which the comment was created.</param>
    public Comment(Guid threadId, Guid authorId, string body, DateTime createdAtUtc)
    {
        ValidateBody(body);

        Id = Guid.NewGuid();
        ThreadId = threadId;
        AuthorId = authorId;
        Body = body;
        CreatedAtUtc = createdAtUtc;
        Status = CommentStatus.PendingAnalysis;
        PublishedAtUtc = null;
        AnalysisLabel = null;
        AnalysisScore = null;
        AnalyzedAtUtc = null;
    }

    /// <summary>
    /// Gets the primary key of the comment.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the identifier of the owning forum thread.
    /// </summary>
    public Guid ThreadId { get; private set; }

    /// <summary>
    /// Gets the identifier of the authoring user.
    /// </summary>
    public Guid AuthorId { get; private set; }

    /// <summary>
    /// Gets the comment text.
    /// </summary>
    public string Body { get; private set; }

    /// <summary>
    /// Gets the current lifecycle status of the comment.
    /// </summary>
    public CommentStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC instant at which the comment was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the UTC instant at which the comment became publicly visible, or <c>null</c> if not visible.
    /// </summary>
    public DateTime? PublishedAtUtc { get; private set; }

    /// <summary>
    /// Gets the toxicity label assigned by analysis, or <c>null</c> while pending analysis.
    /// </summary>
    public ToxicityLabel? AnalysisLabel { get; private set; }

    /// <summary>
    /// Gets the toxic-class probability assigned by analysis, or <c>null</c> while pending analysis.
    /// </summary>
    public float? AnalysisScore { get; private set; }

    /// <summary>
    /// Gets the UTC instant at which analysis completed, or <c>null</c> while pending analysis.
    /// </summary>
    public DateTime? AnalyzedAtUtc { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the comment is publicly visible.
    /// </summary>
    public bool IsPubliclyVisible => Status == CommentStatus.Published || Status == CommentStatus.ApprovedByModerator;

    /// <summary>
    /// Applies an analysis result, routing the comment from <see cref="CommentStatus.PendingAnalysis"/> to its post-analysis state.
    /// </summary>
    /// <param name="result">The analysis result selecting the target state.</param>
    /// <param name="nowUtc">The UTC instant to record for analysis and publication timestamps.</param>
    public void ApplyAnalysis(AnalysisResult result, DateTime nowUtc)
    {
        ArgumentNullException.ThrowIfNull(result);

        CommentStatus target = result.Label == ToxicityLabel.Clean
            ? CommentStatus.Published
            : CommentStatus.FlaggedForReview;

        GuardTransition(target);

        Status = target;
        AnalysisLabel = result.Label;
        AnalysisScore = result.Score;
        AnalyzedAtUtc = nowUtc;
        PublishedAtUtc = target == CommentStatus.Published ? nowUtc : null;
    }

    /// <summary>
    /// Approves a flagged comment, transitioning it to <see cref="CommentStatus.ApprovedByModerator"/> and publishing it.
    /// </summary>
    /// <param name="nowUtc">The UTC instant to record as the publication timestamp.</param>
    public void Approve(DateTime nowUtc)
    {
        GuardTransition(CommentStatus.ApprovedByModerator);

        Status = CommentStatus.ApprovedByModerator;
        PublishedAtUtc = nowUtc;
    }

    /// <summary>
    /// Rejects a flagged comment, transitioning it to <see cref="CommentStatus.RejectedByModerator"/> and keeping it hidden.
    /// </summary>
    public void Reject()
    {
        GuardTransition(CommentStatus.RejectedByModerator);

        Status = CommentStatus.RejectedByModerator;
        PublishedAtUtc = null;
    }

    private void GuardTransition(CommentStatus target)
    {
        if (!CommentStatusTransitions.IsAllowed(Status, target))
        {
            throw new InvalidCommentTransitionException(Status, target);
        }
    }

    private static void ValidateBody(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Comment body must be non-null and non-whitespace.", nameof(body));
        }

        if (body.Length > MaxBodyLength)
        {
            throw new ArgumentException($"Comment body must not exceed {MaxBodyLength} characters.", nameof(body));
        }
    }
}
