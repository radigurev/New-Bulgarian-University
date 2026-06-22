using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;

namespace ForumGuard.Tests.Builders;

/// <summary>
/// Builds <see cref="Comment"/> instances in known lifecycle states for test scenarios.
/// <para>Default values produce a valid, freshly constructed comment in <see cref="CommentStatus.PendingAnalysis"/>.</para>
/// </summary>
public sealed class CommentBuilder
{
    private static readonly DateTime DefaultCreatedAtUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

    private Guid _threadId = Guid.NewGuid();
    private Guid _authorId = Guid.NewGuid();
    private string _body = "A valid comment body.";
    private DateTime _createdAtUtc = DefaultCreatedAtUtc;

    /// <summary>
    /// Creates a new builder seeded with valid default values.
    /// </summary>
    /// <returns>A new <see cref="CommentBuilder"/> instance.</returns>
    public static CommentBuilder Create() => new();

    /// <summary>
    /// Overrides the owning thread identifier.
    /// </summary>
    /// <param name="threadId">The thread identifier to use.</param>
    /// <returns>The same builder instance for chaining.</returns>
    public CommentBuilder WithThreadId(Guid threadId)
    {
        _threadId = threadId;
        return this;
    }

    /// <summary>
    /// Overrides the authoring user identifier.
    /// </summary>
    /// <param name="authorId">The author identifier to use.</param>
    /// <returns>The same builder instance for chaining.</returns>
    public CommentBuilder WithAuthorId(Guid authorId)
    {
        _authorId = authorId;
        return this;
    }

    /// <summary>
    /// Overrides the comment body text.
    /// </summary>
    /// <param name="body">The body text to use.</param>
    /// <returns>The same builder instance for chaining.</returns>
    public CommentBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    /// <summary>
    /// Overrides the creation timestamp.
    /// </summary>
    /// <param name="createdAtUtc">The UTC creation instant to use.</param>
    /// <returns>The same builder instance for chaining.</returns>
    public CommentBuilder WithCreatedAtUtc(DateTime createdAtUtc)
    {
        _createdAtUtc = createdAtUtc;
        return this;
    }

    /// <summary>
    /// Builds a freshly constructed comment in <see cref="CommentStatus.PendingAnalysis"/>.
    /// </summary>
    /// <returns>A new <see cref="Comment"/> instance.</returns>
    public Comment Build() => new(_threadId, _authorId, _body, _createdAtUtc);

    /// <summary>
    /// Builds a comment and advances it to <see cref="CommentStatus.Published"/> via a clean analysis result.
    /// </summary>
    /// <param name="analyzedAtUtc">The UTC instant recorded for analysis and publication.</param>
    /// <param name="score">The clean-class score to record.</param>
    /// <returns>A published <see cref="Comment"/> instance.</returns>
    public Comment BuildPublished(DateTime analyzedAtUtc, float score = 0.1f)
    {
        Comment comment = Build();
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, score), analyzedAtUtc);
        return comment;
    }

    /// <summary>
    /// Builds a comment and advances it to <see cref="CommentStatus.FlaggedForReview"/> via a toxic analysis result.
    /// </summary>
    /// <param name="analyzedAtUtc">The UTC instant recorded for analysis.</param>
    /// <param name="score">The toxic-class score to record.</param>
    /// <returns>A flagged <see cref="Comment"/> instance.</returns>
    public Comment BuildFlagged(DateTime analyzedAtUtc, float score = 0.9f)
    {
        Comment comment = Build();
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, score), analyzedAtUtc);
        return comment;
    }

    /// <summary>
    /// Builds a flagged comment that a moderator has approved (<see cref="CommentStatus.ApprovedByModerator"/>).
    /// </summary>
    /// <param name="analyzedAtUtc">The UTC instant recorded for analysis.</param>
    /// <param name="approvedAtUtc">The UTC instant recorded as the publication timestamp.</param>
    /// <returns>An approved <see cref="Comment"/> instance.</returns>
    public Comment BuildApproved(DateTime analyzedAtUtc, DateTime approvedAtUtc)
    {
        Comment comment = BuildFlagged(analyzedAtUtc);
        comment.Approve(approvedAtUtc);
        return comment;
    }

    /// <summary>
    /// Builds a flagged comment that a moderator has rejected (<see cref="CommentStatus.RejectedByModerator"/>).
    /// </summary>
    /// <param name="analyzedAtUtc">The UTC instant recorded for analysis.</param>
    /// <returns>A rejected <see cref="Comment"/> instance.</returns>
    public Comment BuildRejected(DateTime analyzedAtUtc)
    {
        Comment comment = BuildFlagged(analyzedAtUtc);
        comment.Reject();
        return comment;
    }
}
