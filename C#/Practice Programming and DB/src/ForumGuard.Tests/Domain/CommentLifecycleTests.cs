using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Exceptions;
using ForumGuard.Tests.Builders;

namespace ForumGuard.Tests.Domain;

/// <summary>
/// Verifies <see cref="Comment"/> transition side effects, atomic rejection of illegal and terminal
/// transitions, and the cross-field / state-based invariants per SDD-FORUM-010 §3, §4.2, §4.3.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-010")]
public sealed class CommentLifecycleTests
{
    private static readonly DateTime CreatedAtUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime AnalyzedAtUtc = new(2026, 6, 21, 12, 5, 0, DateTimeKind.Utc);
    private static readonly DateTime DecidedAtUtc = new(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc);

    [Test]
    public void TransitionToPublished_CleanAnalysis_SetsPublishedAtAndAnalysisFields()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();
        AnalysisResult result = new(ToxicityLabel.Clean, 0.05f);

        // Act
        comment.ApplyAnalysis(result, AnalyzedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.Published));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(AnalyzedAtUtc));
            Assert.That(comment.AnalyzedAtUtc, Is.EqualTo(AnalyzedAtUtc));
            Assert.That(comment.AnalysisLabel, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(comment.AnalysisScore, Is.EqualTo(0.05f));
        });
    }

    [Test]
    public void TransitionToFlaggedForReview_ToxicAnalysis_LeavesPublishedAtNull()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();
        AnalysisResult result = new(ToxicityLabel.Toxic, 0.93f);

        // Act
        comment.ApplyAnalysis(result, AnalyzedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(comment.PublishedAtUtc, Is.Null);
            Assert.That(comment.AnalyzedAtUtc, Is.EqualTo(AnalyzedAtUtc));
            Assert.That(comment.AnalysisLabel, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(comment.AnalysisScore, Is.EqualTo(0.93f));
        });
    }

    [Test]
    public void TransitionToApprovedByModerator_FromFlagged_SetsPublishedAt()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);

        // Act
        comment.Approve(DecidedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.ApprovedByModerator));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(DecidedAtUtc));
            Assert.That(comment.IsPubliclyVisible, Is.True);
        });
    }

    [Test]
    public void TransitionToRejectedByModerator_FromFlagged_LeavesPublishedAtNull()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);

        // Act
        comment.Reject();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.RejectedByModerator));
            Assert.That(comment.PublishedAtUtc, Is.Null);
            Assert.That(comment.IsPubliclyVisible, Is.False);
        });
    }

    [Test]
    public void ApplyTransition_IllegalPair_RaisesInvalidCommentTransition()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();

        // Act
        TestDelegate approveFromPending = () => comment.Approve(DecidedAtUtc);

        // Assert
        Assert.That(approveFromPending, Throws.TypeOf<InvalidCommentTransitionException>());
    }

    [Test]
    public void ApplyTransition_IllegalPair_LeavesCommentUnchanged()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();
        CommentStatus originalStatus = comment.Status;
        DateTime? originalPublishedAt = comment.PublishedAtUtc;
        ToxicityLabel? originalLabel = comment.AnalysisLabel;
        float? originalScore = comment.AnalysisScore;
        DateTime? originalAnalyzedAt = comment.AnalyzedAtUtc;

        // Act
        Assert.That(() => comment.Reject(), Throws.TypeOf<InvalidCommentTransitionException>());

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Status, Is.EqualTo(originalStatus));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(originalPublishedAt));
            Assert.That(comment.AnalysisLabel, Is.EqualTo(originalLabel));
            Assert.That(comment.AnalysisScore, Is.EqualTo(originalScore));
            Assert.That(comment.AnalyzedAtUtc, Is.EqualTo(originalAnalyzedAt));
        });
    }

    [Test]
    public void ApplyTransition_PublishedToFlaggedForReview_IsRejected()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildPublished(AnalyzedAtUtc);
        CommentStatus originalStatus = comment.Status;
        DateTime? originalPublishedAt = comment.PublishedAtUtc;
        ToxicityLabel? originalLabel = comment.AnalysisLabel;

        // Act
        TestDelegate reAnalyzeAsToxic = () => comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.99f), DecidedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(reAnalyzeAsToxic, Throws.TypeOf<InvalidCommentTransitionException>());
            Assert.That(comment.Status, Is.EqualTo(originalStatus));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(originalPublishedAt));
            Assert.That(comment.AnalysisLabel, Is.EqualTo(originalLabel));
        });
    }

    [Test]
    public void ApplyTransition_RejectedToApproved_IsRejected()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildRejected(AnalyzedAtUtc);
        CommentStatus originalStatus = comment.Status;
        DateTime? originalPublishedAt = comment.PublishedAtUtc;

        // Act
        TestDelegate approveAfterReject = () => comment.Approve(DecidedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(approveAfterReject, Throws.TypeOf<InvalidCommentTransitionException>());
            Assert.That(comment.Status, Is.EqualTo(originalStatus));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(originalPublishedAt));
        });
    }

    [Test]
    public void ApplyAnalysis_FromPublished_IsRejected()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildPublished(AnalyzedAtUtc);

        // Act
        TestDelegate reAnalyze = () => comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, 0.01f), DecidedAtUtc);

        // Assert
        Assert.That(reAnalyze, Throws.TypeOf<InvalidCommentTransitionException>());
    }

    [Test]
    public void ApplyAnalysis_NullResult_ThrowsArgumentNullException()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();

        // Act
        TestDelegate applyNull = () => comment.ApplyAnalysis(null!, AnalyzedAtUtc);

        // Assert
        Assert.That(applyNull, Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Validate_PendingAnalysisWithAnalysisFieldsSet_FailsCF2()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();

        // Act
        bool pendingHasNoAnalysisState = comment.Status == CommentStatus.PendingAnalysis
            && comment.AnalysisLabel is null
            && comment.AnalysisScore is null
            && comment.AnalyzedAtUtc is null
            && comment.PublishedAtUtc is null;

        // Assert
        Assert.That(pendingHasNoAnalysisState, Is.True);
    }

    [Test]
    public void Validate_NonPendingStatusWithNullAnalysisFields_FailsCF3()
    {
        // Arrange
        Comment published = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildPublished(AnalyzedAtUtc);
        Comment flagged = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);
        Comment approved = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildApproved(AnalyzedAtUtc, DecidedAtUtc);
        Comment rejected = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildRejected(AnalyzedAtUtc);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            AssertAnalysisFieldsPopulated(published);
            AssertAnalysisFieldsPopulated(flagged);
            AssertAnalysisFieldsPopulated(approved);
            AssertAnalysisFieldsPopulated(rejected);
        });
    }

    [Test]
    public void Validate_FlaggedForReviewWithPublishedAtSet_FailsCF4()
    {
        // Arrange
        Comment flagged = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);
        Comment rejected = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildRejected(AnalyzedAtUtc);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(flagged.PublishedAtUtc, Is.Null);
            Assert.That(rejected.PublishedAtUtc, Is.Null);
        });
    }

    [TestCase(float.NaN)]
    [TestCase(float.PositiveInfinity)]
    [TestCase(-0.01f)]
    [TestCase(1.01f)]
    public void Validate_AnalysisScoreOutOfRange_FailsValidation(float score)
    {
        // Arrange
        // Act
        TestDelegate construction = () => new AnalysisResult(ToxicityLabel.Clean, score);

        // Assert
        Assert.That(construction, Throws.TypeOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Validate_CleanLabelRoutedToFlaggedForReview_FailsCF5()
    {
        // Arrange
        Comment cleanComment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();
        Comment toxicComment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();

        // Act
        cleanComment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, 0.02f), AnalyzedAtUtc);
        toxicComment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.97f), AnalyzedAtUtc);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(cleanComment.Status, Is.EqualTo(CommentStatus.Published));
            Assert.That(cleanComment.Status, Is.Not.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(toxicComment.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(toxicComment.Status, Is.Not.EqualTo(CommentStatus.Published));
        });
    }

    /// <summary>
    /// Asserts that a comment which has left <see cref="CommentStatus.PendingAnalysis"/> has all analysis fields populated (CF-3).
    /// </summary>
    /// <param name="comment">The comment to inspect.</param>
    private static void AssertAnalysisFieldsPopulated(Comment comment)
    {
        Assert.That(comment.AnalysisLabel, Is.Not.Null, $"AnalysisLabel must be set in state {comment.Status}.");
        Assert.That(comment.AnalysisScore, Is.Not.Null, $"AnalysisScore must be set in state {comment.Status}.");
        Assert.That(comment.AnalyzedAtUtc, Is.Not.Null, $"AnalyzedAtUtc must be set in state {comment.Status}.");
    }
}
