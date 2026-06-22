using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Lifecycle;

namespace ForumGuard.Tests.Domain;

/// <summary>
/// Verifies the pure transition guard <see cref="CommentStatusTransitions.IsAllowed"/> against the
/// authoritative transition table in SDD-FORUM-010 §3.3, §3.4, §3.5.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-010")]
public sealed class CommentStatusTransitionsTests
{
    private static readonly CommentStatus[] AllStates =
    [
        CommentStatus.PendingAnalysis,
        CommentStatus.Published,
        CommentStatus.FlaggedForReview,
        CommentStatus.ApprovedByModerator,
        CommentStatus.RejectedByModerator
    ];

    [Test]
    public void IsAllowed_PendingAnalysisToPublished_ReturnsTrue()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.PendingAnalysis, CommentStatus.Published);

        // Assert
        Assert.That(allowed, Is.True);
    }

    [Test]
    public void IsAllowed_PendingAnalysisToFlaggedForReview_ReturnsTrue()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.PendingAnalysis, CommentStatus.FlaggedForReview);

        // Assert
        Assert.That(allowed, Is.True);
    }

    [Test]
    public void IsAllowed_FlaggedForReviewToApprovedByModerator_ReturnsTrue()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.FlaggedForReview, CommentStatus.ApprovedByModerator);

        // Assert
        Assert.That(allowed, Is.True);
    }

    [Test]
    public void IsAllowed_FlaggedForReviewToRejectedByModerator_ReturnsTrue()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.FlaggedForReview, CommentStatus.RejectedByModerator);

        // Assert
        Assert.That(allowed, Is.True);
    }

    [Test]
    public void IsAllowed_FlaggedForReviewToPublished_ReturnsFalse()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.FlaggedForReview, CommentStatus.Published);

        // Assert
        Assert.That(allowed, Is.False);
    }

    [Test]
    public void IsAllowed_PendingAnalysisToApprovedByModerator_ReturnsFalse()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.PendingAnalysis, CommentStatus.ApprovedByModerator);

        // Assert
        Assert.That(allowed, Is.False);
    }

    [Test]
    public void IsAllowed_PendingAnalysisToRejectedByModerator_ReturnsFalse()
    {
        // Arrange
        // Act
        bool allowed = CommentStatusTransitions.IsAllowed(CommentStatus.PendingAnalysis, CommentStatus.RejectedByModerator);

        // Assert
        Assert.That(allowed, Is.False);
    }

    [Test]
    public void IsAllowed_AnyStateToPendingAnalysis_ReturnsFalse()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            foreach (CommentStatus from in AllStates)
            {
                Assert.That(
                    CommentStatusTransitions.IsAllowed(from, CommentStatus.PendingAnalysis),
                    Is.False,
                    $"Transition {from} -> PendingAnalysis must be rejected.");
            }
        });
    }

    [Test]
    public void IsAllowed_SelfTransition_ReturnsFalseForEveryState()
    {
        // Arrange
        // Act
        // Assert
        Assert.Multiple(() =>
        {
            foreach (CommentStatus state in AllStates)
            {
                Assert.That(
                    CommentStatusTransitions.IsAllowed(state, state),
                    Is.False,
                    $"Self-transition {state} -> {state} must be rejected.");
            }
        });
    }

    [Test]
    public void IsAllowed_FromPublished_ReturnsFalseForEveryTarget()
    {
        // Arrange
        // Act
        // Assert
        AssertNoOutgoingTransitions(CommentStatus.Published);
    }

    [Test]
    public void IsAllowed_FromApprovedByModerator_ReturnsFalseForEveryTarget()
    {
        // Arrange
        // Act
        // Assert
        AssertNoOutgoingTransitions(CommentStatus.ApprovedByModerator);
    }

    [Test]
    public void IsAllowed_FromRejectedByModerator_ReturnsFalseForEveryTarget()
    {
        // Arrange
        // Act
        // Assert
        AssertNoOutgoingTransitions(CommentStatus.RejectedByModerator);
    }

    [Test]
    public void IsAllowed_ExactlyFourRuntimeTransitions_ReturnTrue()
    {
        // Arrange
        int allowedCount = 0;

        // Act
        foreach (CommentStatus from in AllStates)
        {
            foreach (CommentStatus to in AllStates)
            {
                if (CommentStatusTransitions.IsAllowed(from, to))
                {
                    allowedCount++;
                }
            }
        }

        // Assert
        Assert.That(allowedCount, Is.EqualTo(4));
    }

    /// <summary>
    /// Asserts that the given terminal state has no allowed outgoing transition to any state.
    /// </summary>
    /// <param name="terminal">The terminal state under test.</param>
    private static void AssertNoOutgoingTransitions(CommentStatus terminal)
    {
        Assert.Multiple(() =>
        {
            foreach (CommentStatus to in AllStates)
            {
                Assert.That(
                    CommentStatusTransitions.IsAllowed(terminal, to),
                    Is.False,
                    $"Terminal state {terminal} must have no outgoing transition to {to}.");
            }
        });
    }
}
