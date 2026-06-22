using ForumGuard.Application.Comments;
using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation;
using ForumGuard.Application.Options;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Tests.Application.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ForumGuard.Tests.Application.Comments;

/// <summary>
/// Verifies comment submission orchestration: gating, validation, PendingAnalysis creation, synchronous
/// pipeline routing, fail-safe flagging, and persistence per SDD-FORUM-001 §2 and §3.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-001")]
public sealed class CommentSubmissionServiceTests
{
    private static readonly DateTime NowUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
    private static readonly Guid ThreadId = Guid.NewGuid();
    private static readonly Guid AuthorId = Guid.NewGuid();

    private Mock<IForumThreadRepository> _threadRepository = null!;
    private Mock<ICommentRepository> _commentRepository = null!;
    private Mock<ICommentModerationPipeline> _pipeline = null!;
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private FixedClock _clock = null!;
    private StaticOptionsMonitor<ModerationOptions> _options = null!;

    [SetUp]
    public void SetUp()
    {
        _threadRepository = new Mock<IForumThreadRepository>();
        _commentRepository = new Mock<ICommentRepository>();
        _pipeline = new Mock<ICommentModerationPipeline>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _clock = new FixedClock(NowUtc);
        _options = new StaticOptionsMonitor<ModerationOptions>(new ModerationOptions
        {
            ToxicityThreshold = 0.5f,
            ModelPath = "model.zip",
            ProfanityListPath = "profanity.txt",
            MaxCommentLength = 4000
        });

        _threadRepository
            .Setup(repository => repository.GetByIdAsync(ThreadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ForumThread("Topic", Guid.NewGuid(), NowUtc));

        _unitOfWork
            .Setup(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    private CommentSubmissionService CreateService() => new(
        _threadRepository.Object,
        _commentRepository.Object,
        _pipeline.Object,
        _unitOfWork.Object,
        _clock,
        _options,
        NullLogger<CommentSubmissionService>.Instance);

    private static SubmitCommentRequest ValidRequest(string body = "A clean comment body.") =>
        new(ThreadId, AuthorId, AuthorId, body, IsAuthorActive: true);

    private void SetupPipelineVerdict(ModerationVerdict verdict) =>
        _pipeline
            .Setup(pipeline => pipeline.EvaluateAsync(It.IsAny<CommentModerationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(verdict);

    [Test]
    public async Task SubmitCommentAsync_CleanResult_SetsStatusPublishedAndPublishedAtUtc()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        SetupPipelineVerdict(ModerationVerdict.Clean(0.2f));
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value!.Status, Is.EqualTo(CommentStatus.Published));
            Assert.That(added!.Status, Is.EqualTo(CommentStatus.Published));
            Assert.That(added.PublishedAtUtc, Is.EqualTo(NowUtc));
        });
    }

    [Test]
    public async Task SubmitCommentAsync_CleanResult_PersistsAnalysisLabelScoreAndAnalyzedAtUtc()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        SetupPipelineVerdict(ModerationVerdict.Clean(0.2f));
        CommentSubmissionService service = CreateService();

        // Act
        await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(added!.AnalysisLabel, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(added.AnalysisScore, Is.EqualTo(0.2f));
            Assert.That(added.AnalyzedAtUtc, Is.EqualTo(NowUtc));
        });
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SubmitCommentAsync_ToxicResult_SetsStatusFlaggedForReviewAndLeavesPublishedAtUtcNull()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        SetupPipelineVerdict(ModerationVerdict.Flagged(ProfanityPreFilterHandlerName, 0.95f));
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Value!.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(added!.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(added.PublishedAtUtc, Is.Null);
        });
    }

    [Test]
    public async Task SubmitCommentAsync_PipelineThrows_FlagsForReviewAndDoesNotPublish()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        _pipeline
            .Setup(pipeline => pipeline.EvaluateAsync(It.IsAny<CommentModerationContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("pipeline failure"));
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(added!.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(added.PublishedAtUtc, Is.Null);
        });
    }

    [Test]
    public async Task SubmitCommentAsync_PipelineThrows_PersistsCommentAndLeavesPublishedAtUtcNull()
    {
        // Arrange
        _pipeline
            .Setup(pipeline => pipeline.EvaluateAsync(It.IsAny<CommentModerationContext>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("pipeline failure"));
        CommentSubmissionService service = CreateService();

        // Act
        await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Once);
        _unitOfWork.Verify(unit => unit.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SubmitCommentAsync_ScoreExactlyAtThreshold_ClassifiesToxicAndFlagsForReview()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        SetupPipelineVerdict(ModerationVerdict.Flagged(MlToxicityHandlerName, 0.5f));
        CommentSubmissionService service = CreateService();

        // Act
        await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.That(added!.Status, Is.EqualTo(CommentStatus.FlaggedForReview));
    }

    [Test]
    public async Task SubmitCommentAsync_NewComment_StartsInPendingAnalysisBeforeAnalysis()
    {
        // Arrange
        CommentStatus statusAtAnalysisTime = CommentStatus.Published;
        _pipeline
            .Setup(pipeline => pipeline.EvaluateAsync(It.IsAny<CommentModerationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ModerationVerdict.Clean(0.1f));
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        CommentSubmissionService service = CreateService();

        // Act
        await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(added, Is.Not.Null);
            Assert.That(statusAtAnalysisTime, Is.Not.EqualTo(CommentStatus.PendingAnalysis));
            Assert.That(added!.CreatedAtUtc, Is.EqualTo(NowUtc));
        });
    }

    [TestCase("")]
    [TestCase("   ")]
    public async Task SubmitCommentAsync_EmptyOrWhitespaceBody_ReturnsValidationFailureAndCreatesNoComment(string body)
    {
        // Arrange
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest(body), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Status, Is.EqualTo(ResultStatus.Validation));
            Assert.That(result.IsSuccess, Is.False);
        });
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Never);
    }

    [Test]
    public async Task SubmitCommentAsync_BodyExceedsMaxCommentLength_ReturnsValidationFailure()
    {
        // Arrange
        StaticOptionsMonitor<ModerationOptions> shortLimit = new(new ModerationOptions
        {
            ToxicityThreshold = 0.5f,
            ModelPath = "model.zip",
            ProfanityListPath = "profanity.txt",
            MaxCommentLength = 10
        });
        _options = shortLimit;
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(
            ValidRequest("this body is far longer than ten characters"),
            CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.Validation));
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Never);
    }

    [Test]
    public async Task SubmitCommentAsync_BodyTrimmedBeforeLengthCheck_AcceptsBodyWithinLimit()
    {
        // Arrange
        Comment? added = null;
        _commentRepository.Setup(repository => repository.Add(It.IsAny<Comment>())).Callback<Comment>(comment => added = comment);
        SetupPipelineVerdict(ModerationVerdict.Clean(0.1f));
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest("   trimmed   "), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(added!.Body, Is.EqualTo("trimmed"));
        });
    }

    [Test]
    public async Task SubmitCommentAsync_InactiveAccount_ReturnsForbiddenAndCreatesNoComment()
    {
        // Arrange
        SubmitCommentRequest request = new(ThreadId, AuthorId, AuthorId, "A clean comment.", IsAuthorActive: false);
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.Forbidden));
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Never);
    }

    [Test]
    public async Task SubmitCommentAsync_AuthorIdNotSignedInUser_ReturnsForbidden()
    {
        // Arrange
        SubmitCommentRequest request = new(ThreadId, AuthorId, Guid.NewGuid(), "A clean comment.", IsAuthorActive: true);
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(request, CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.Forbidden));
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Never);
    }

    [Test]
    public async Task SubmitCommentAsync_NonExistentThread_ReturnsNotFoundAndCreatesNoComment()
    {
        // Arrange
        _threadRepository
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ForumThread?)null);
        CommentSubmissionService service = CreateService();

        // Act
        Result<SubmitCommentResult> result = await service.SubmitCommentAsync(ValidRequest(), CancellationToken.None);

        // Assert
        Assert.That(result.Status, Is.EqualTo(ResultStatus.NotFound));
        _commentRepository.Verify(repository => repository.Add(It.IsAny<Comment>()), Times.Never);
    }

    private const string ProfanityPreFilterHandlerName = "ProfanityPreFilterHandler";
    private const string MlToxicityHandlerName = "MlToxicityHandler";
}
