using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Tests.Builders;

namespace ForumGuard.Tests.Domain;

/// <summary>
/// Verifies the visibility invariant <see cref="Comment.IsPubliclyVisible"/> per SDD-FORUM-010 §3.6.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-010")]
public sealed class CommentVisibilityTests
{
    private static readonly DateTime CreatedAtUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime AnalyzedAtUtc = new(2026, 6, 21, 12, 5, 0, DateTimeKind.Utc);
    private static readonly DateTime DecidedAtUtc = new(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc);

    [Test]
    public void IsPubliclyVisible_StatusPublished_ReturnsTrue()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildPublished(AnalyzedAtUtc);

        // Act
        bool visible = comment.IsPubliclyVisible;

        // Assert
        Assert.That(visible, Is.True);
    }

    [Test]
    public void IsPubliclyVisible_StatusApprovedByModerator_ReturnsTrue()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildApproved(AnalyzedAtUtc, DecidedAtUtc);

        // Act
        bool visible = comment.IsPubliclyVisible;

        // Assert
        Assert.That(visible, Is.True);
    }

    [Test]
    public void IsPubliclyVisible_StatusPendingAnalysis_ReturnsFalse()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();

        // Act
        bool visible = comment.IsPubliclyVisible;

        // Assert
        Assert.That(visible, Is.False);
    }

    [Test]
    public void IsPubliclyVisible_StatusFlaggedForReview_ReturnsFalse()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);

        // Act
        bool visible = comment.IsPubliclyVisible;

        // Assert
        Assert.That(visible, Is.False);
    }

    [Test]
    public void IsPubliclyVisible_StatusRejectedByModerator_ReturnsFalse()
    {
        // Arrange
        Comment comment = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildRejected(AnalyzedAtUtc);

        // Act
        bool visible = comment.IsPubliclyVisible;

        // Assert
        Assert.That(visible, Is.False);
    }

    [Test]
    public void IsPubliclyVisible_True_ImpliesPublishedAtNotNull()
    {
        // Arrange
        Comment published = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildPublished(AnalyzedAtUtc);
        Comment approved = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildApproved(AnalyzedAtUtc, DecidedAtUtc);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(published.IsPubliclyVisible, Is.True);
            Assert.That(published.PublishedAtUtc, Is.Not.Null);
            Assert.That(approved.IsPubliclyVisible, Is.True);
            Assert.That(approved.PublishedAtUtc, Is.Not.Null);
        });
    }

    [Test]
    public void IsPubliclyVisible_False_ImpliesPublishedAtNull()
    {
        // Arrange
        Comment pending = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).Build();
        Comment flagged = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildFlagged(AnalyzedAtUtc);
        Comment rejected = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc).BuildRejected(AnalyzedAtUtc);

        // Act
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(pending.PublishedAtUtc, Is.Null);
            Assert.That(flagged.PublishedAtUtc, Is.Null);
            Assert.That(rejected.PublishedAtUtc, Is.Null);
        });
    }
}
