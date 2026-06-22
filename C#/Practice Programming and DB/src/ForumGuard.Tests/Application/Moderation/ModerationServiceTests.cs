using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation;
using ForumGuard.Application.Moderation.Dtos;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Tests.Application.Fakes;
using ForumGuard.Tests.Builders;
using Moq;

namespace ForumGuard.Tests.Application.Moderation;

/// <summary>
/// Verifies the moderator review workflow: queue listing, Approve/Reject transitions, the audit-row write,
/// conflict handling for non-flagged comments, and validation per SDD-FORUM-002 §2 and §3.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-002")]
public sealed class ModerationServiceTests
{
    private static readonly DateTime AnalyzedAtUtc = new(2026, 6, 21, 12, 5, 0, DateTimeKind.Utc);
    private static readonly DateTime DecidedAtUtc = new(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc);
    private static readonly Guid ModeratorId = Guid.NewGuid();

    private Mock<ICommentRepository> _commentRepository = null!;
    private Mock<IModerationDecisionRepository> _decisionRepository = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private FixedClock _clock = null!;

    [SetUp]
    public void SetUp()
    {
        _commentRepository = new Mock<ICommentRepository>();
        _decisionRepository = new Mock<IModerationDecisionRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _clock = new FixedClock(DecidedAtUtc);

        _unitOfWork
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private ModerationService CreateService() => new(
        _commentRepository.Object,
        _decisionRepository.Object,
        _unitOfWork.Object,
        _clock);

    private static Comment FlaggedComment() =>
        CommentBuilder.Create().BuildFlagged(AnalyzedAtUtc);

    private void SetupComment(Comment comment) =>
        _commentRepository
            .Setup(repository => repository.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

    private static ModerationDecisionRequest Request(Comment comment, ModerationOutcome decision, string? reason = null) =>
        new(comment.Id, ModeratorId, decision, reason);

    [Test]
    public async Task GetQueue_OnlyFlaggedForReviewComments_ReturnsThoseComments()
    {
        // Arrange
        Comment flaggedA = CommentBuilder.Create().WithCreatedAtUtc(AnalyzedAtUtc).BuildFlagged(AnalyzedAtUtc);
        Comment flaggedB = CommentBuilder.Create().WithCreatedAtUtc(AnalyzedAtUtc).BuildFlagged(AnalyzedAtUtc);
        _commentRepository
            .Setup(repository => repository.GetFlaggedQueueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([flaggedA, flaggedB]);
        ModerationService service = CreateService();

        // Act
        IReadOnlyList<QueuedCommentDto> queue = await service.GetQueueAsync(CancellationToken.None);

        // Assert
        Assert.That(queue, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetQueue_MapsCommentFieldsIntoDto()
    {
        // Arrange
        Comment flagged = CommentBuilder.Create().WithCreatedAtUtc(AnalyzedAtUtc).BuildFlagged(AnalyzedAtUtc, 0.91f);
        _commentRepository
            .Setup(repository => repository.GetFlaggedQueueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([flagged]);
        ModerationService service = CreateService();

        // Act
        IReadOnlyList<QueuedCommentDto> queue = await service.GetQueueAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(queue[0].CommentId, Is.EqualTo(flagged.Id));
            Assert.That(queue[0].AnalysisLabel, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(queue[0].AnalysisScore, Is.EqualTo(0.91f));
        });
    }

    [Test]
    public async Task Approve_FlaggedComment_TransitionsToApprovedByModeratorAndSetsPublishedAtUtc()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.ApprovedByModerator));
            Assert.That(comment.PublishedAtUtc, Is.EqualTo(DecidedAtUtc));
        });
    }

    [Test]
    public async Task Approve_FlaggedComment_WritesModerationDecisionWithApprovedOutcome()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(written, Is.Not.Null);
            Assert.That(written!.Decision, Is.EqualTo(ModerationOutcome.Approved));
            Assert.That(written.ModeratorId, Is.EqualTo(ModeratorId));
            Assert.That(written.DecidedAtUtc, Is.EqualTo(DecidedAtUtc));
        });
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Reject_FlaggedComment_TransitionsToRejectedByModeratorAndLeavesPublishedAtUtcNull()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(Request(comment, ModerationOutcome.Rejected), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.RejectedByModerator));
            Assert.That(comment.PublishedAtUtc, Is.Null);
        });
    }

    [Test]
    public async Task Reject_FlaggedComment_WritesModerationDecisionWithRejectedOutcome()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Rejected), CancellationToken.None);

        // Assert
        Assert.That(written!.Decision, Is.EqualTo(ModerationOutcome.Rejected));
    }

    [Test]
    public async Task Decide_AnyOutcome_RecordsActingModeratorIdAndDecidedAtUtc()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(written!.ModeratorId, Is.EqualTo(ModeratorId));
            Assert.That(written.DecidedAtUtc, Is.EqualTo(DecidedAtUtc));
            Assert.That(written.CommentId, Is.EqualTo(comment.Id));
        });
    }

    [Test]
    public async Task Approve_CommentNotInFlaggedForReview_ReturnsConflictAndDoesNotTransition()
    {
        // Arrange
        Comment published = CommentBuilder.Create().BuildPublished(AnalyzedAtUtc);
        SetupComment(published);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(Request(published, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Conflict));
            Assert.That(published.Status, Is.EqualTo(CommentStatus.Published));
        });
        _decisionRepository.Verify(repository => repository.Add(It.IsAny<ModerationDecision>()), Times.Never);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Reject_CommentNotInFlaggedForReview_ReturnsConflictAndDoesNotTransition()
    {
        // Arrange
        Comment approved = CommentBuilder.Create().BuildApproved(AnalyzedAtUtc, DecidedAtUtc);
        SetupComment(approved);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(Request(approved, ModerationOutcome.Rejected), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Conflict));
            Assert.That(approved.Status, Is.EqualTo(CommentStatus.ApprovedByModerator));
        });
        _decisionRepository.Verify(repository => repository.Add(It.IsAny<ModerationDecision>()), Times.Never);
    }

    [Test]
    public async Task Decide_CommentIdNotFound_ReturnsNotFound()
    {
        // Arrange
        _commentRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);
        ModerationDecisionRequest request = new(Guid.NewGuid(), ModeratorId, ModerationOutcome.Approved);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.NotFound));
    }

    [Test]
    public async Task Decide_UndefinedOutcome_ReturnsValidationFailure()
    {
        // Arrange
        ModerationDecisionRequest request = new(Guid.NewGuid(), ModeratorId, (ModerationOutcome)99);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.Validation));
        _commentRepository.Verify(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Decide_ReasonOverFiveHundredChars_ReturnsValidationFailure()
    {
        // Arrange
        string longReason = new('x', ModerationDecision.MaxReasonLength + 1);
        ModerationDecisionRequest request = new(Guid.NewGuid(), ModeratorId, ModerationOutcome.Rejected, longReason);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.Validation));
    }

    [Test]
    public async Task Decide_ReasonOmitted_StoresReasonAsNull()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.That(written!.Reason, Is.Null);
    }

    [Test]
    public async Task Decide_SuppliedReason_PersistsReasonOnDecision()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Rejected, "Spam content."), CancellationToken.None);

        // Assert
        Assert.That(written!.Reason, Is.EqualTo("Spam content."));
    }

    [Test]
    public async Task Approve_FlaggedComment_UpdatesCommentAndAddsDecisionBeforeSingleSave()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationService service = CreateService();

        // Act
        await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        _commentRepository.Verify(repository => repository.Update(comment), Times.Once);
        _decisionRepository.Verify(repository => repository.Add(It.IsAny<ModerationDecision>()), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Approve_FlaggedComment_ReturnsResultWithTerminalStatusAndDecisionId()
    {
        // Arrange
        Comment comment = FlaggedComment();
        SetupComment(comment);
        ModerationDecision? written = null;
        _decisionRepository.Setup(repository => repository.Add(It.IsAny<ModerationDecision>())).Callback<ModerationDecision>(decision => written = decision);
        ModerationService service = CreateService();

        // Act
        Result<ModerationDecisionResult> result = await service.DecideAsync(Request(comment, ModerationOutcome.Approved), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Status, Is.EqualTo(CommentStatus.ApprovedByModerator));
            Assert.That(result.Value.CommentId, Is.EqualTo(comment.Id));
            Assert.That(result.Value.DecisionId, Is.EqualTo(written!.Id));
        });
    }
}
