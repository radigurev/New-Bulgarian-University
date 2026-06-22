using ForumGuard.Application.Moderation;
using ForumGuard.Application.Moderation.Handlers;
using ForumGuard.Application.Options;
using ForumGuard.Domain.Enums;
using ForumGuard.Tests.Application.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace ForumGuard.Tests.Application.Moderation;

/// <summary>
/// Verifies the moderation Chain-of-Responsibility traversal, short-circuiting, empty-chain default,
/// fail-safe-on-throw, and configurable ordering rules per SDD-FORUM-021.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-021")]
public sealed class CommentModerationPipelineTests
{
    private static readonly Guid CommentId = Guid.NewGuid();

    private static CommentModerationContext CleanContext() =>
        new("a perfectly normal comment", CommentId);

    private static CommentModerationPipeline Pipeline(params ICommentModerationHandler[] handlers) =>
        new(handlers, NullLogger<CommentModerationPipeline>.Instance);

    private static MlToxicityHandler MlHandler(StubCommentAnalyzer analyzer, float threshold)
    {
        ModerationOptions options = new()
        {
            ToxicityThreshold = threshold,
            ModelPath = "model.zip",
            ProfanityListPath = "profanity.txt",
            MaxCommentLength = 4000
        };

        return new MlToxicityHandler(analyzer, new StaticOptionsMonitor<ModerationOptions>(options));
    }

    [Test]
    public async Task EvaluateAsync_CleanText_TraversesWholeChainAndReturnsClean()
    {
        // Arrange
        StubCommentAnalyzer keyword = StubCommentAnalyzer.Clean(0.0f);
        StubCommentAnalyzer ml = StubCommentAnalyzer.Clean(0.2f);
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(keyword),
            MlHandler(ml, 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(keyword.InvocationCount, Is.EqualTo(1));
            Assert.That(ml.InvocationCount, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task EvaluateAsync_CleanText_ReturnsIsFlaggedFalseAndNullFlaggingHandler()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Clean(0.0f)),
            MlHandler(StubCommentAnalyzer.Clean(0.3f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.False);
            Assert.That(verdict.FlaggingHandlerName, Is.Null);
            Assert.That(verdict.Score, Is.EqualTo(0.3f));
        });
    }

    [Test]
    public async Task EvaluateAsync_ProfanityKeyword_FlaggedByProfanityPreFilterHandler()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Toxic(1.0f)),
            MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(verdict.IsFlagged, Is.True);
            Assert.That(verdict.FlaggingHandlerName, Is.EqualTo(nameof(ProfanityPreFilterHandler)));
        });
    }

    [Test]
    public async Task EvaluateAsync_ProfanityKeyword_DoesNotInvokeMlAnalyzer()
    {
        // Arrange
        StubCommentAnalyzer ml = StubCommentAnalyzer.Toxic(1.0f);
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Toxic(1.0f)),
            MlHandler(ml, 0.5f));

        // Act
        await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(ml.InvocationCount, Is.EqualTo(0));
    }

    [Test]
    public async Task EvaluateAsync_ProfanityKeyword_SetsFlaggingHandlerNameToProfanityPreFilter()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Toxic(1.0f)),
            MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(verdict.FlaggingHandlerName, Is.EqualTo(nameof(ProfanityPreFilterHandler)));
    }

    [Test]
    public async Task EvaluateAsync_MlScoreAboveThreshold_FlaggedByMlToxicityHandler()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Clean(0.0f)),
            MlHandler(StubCommentAnalyzer.Toxic(0.8f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.True);
            Assert.That(verdict.FlaggingHandlerName, Is.EqualTo(nameof(MlToxicityHandler)));
            Assert.That(verdict.Score, Is.EqualTo(0.8f));
        });
    }

    [Test]
    public async Task EvaluateAsync_MlScoreEqualToThreshold_FlagsInclusively()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Clean(0.0f)),
            MlHandler(StubCommentAnalyzer.Toxic(0.5f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.True);
            Assert.That(verdict.FlaggingHandlerName, Is.EqualTo(nameof(MlToxicityHandler)));
        });
    }

    [Test]
    public async Task EvaluateAsync_MlScoreBelowThreshold_PassesAndReturnsClean()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Clean(0.0f)),
            MlHandler(StubCommentAnalyzer.Clean(0.49f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.False);
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Clean));
        });
    }

    [Test]
    public async Task EvaluateAsync_EmptyChain_ReturnsCleanWithZeroScore()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline();

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(verdict.IsFlagged, Is.False);
            Assert.That(verdict.Score, Is.EqualTo(0.0f));
            Assert.That(verdict.FlaggingHandlerName, Is.Null);
        });
    }

    [Test]
    public void EvaluateAsync_EmptyChain_DoesNotThrow()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline();

        // Act
        async Task Act() => await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(Act, Throws.Nothing);
    }

    [Test]
    public async Task EvaluateAsync_FirstHandlerFlags_DoesNotRunSubsequentHandlers()
    {
        // Arrange
        RecordingModerationHandler.ResetSequence();
        RecordingModerationHandler first = RecordingModerationHandler.Flagging("First", 100);
        RecordingModerationHandler second = RecordingModerationHandler.Passing("Second", 200);
        CommentModerationPipeline pipeline = Pipeline(first, second);

        // Act
        await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(first.InvocationCount, Is.EqualTo(1));
            Assert.That(second.InvocationCount, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task EvaluateAsync_HandlerThrows_ReturnsFailSafeFlaggedVerdict()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(RecordingModerationHandler.Throwing("Boom", 100));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.True);
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(verdict.FlaggingHandlerName, Is.EqualTo("Boom"));
        });
    }

    [Test]
    public void EvaluateAsync_HandlerThrows_DoesNotPropagateException()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(RecordingModerationHandler.Throwing("Boom", 100));

        // Act
        async Task Act() => await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(Act, Throws.Nothing);
    }

    [Test]
    public async Task EvaluateAsync_HandlerThrows_NeverReturnsCleanVerdict()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(new ThrowingCommentAnalyzer()),
            MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(verdict.Label, Is.Not.EqualTo(ToxicityLabel.Clean));
    }

    [Test]
    public async Task EvaluateAsync_HandlerThrows_StopsChainAndDoesNotRunNextHandler()
    {
        // Arrange
        RecordingModerationHandler.ResetSequence();
        RecordingModerationHandler throwing = RecordingModerationHandler.Throwing("Boom", 100);
        RecordingModerationHandler next = RecordingModerationHandler.Passing("Next", 200);
        CommentModerationPipeline pipeline = Pipeline(throwing, next);

        // Act
        await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(next.InvocationCount, Is.EqualTo(0));
    }

    [Test]
    public async Task EvaluateAsync_NullBody_TreatedAsEmptyAndReturnsClean()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(StubCommentAnalyzer.Clean(0.0f)),
            MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(new CommentModerationContext(null, CommentId), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(verdict.IsFlagged, Is.False);
        });
    }

    [Test]
    public async Task EvaluateAsync_ConfiguredOrderReversed_TraversesInConfiguredOrder()
    {
        // Arrange
        RecordingModerationHandler.ResetSequence();
        RecordingModerationHandler early = RecordingModerationHandler.Passing("Early", 100);
        RecordingModerationHandler late = RecordingModerationHandler.Passing("Late", 200);
        CommentModerationPipeline pipeline = Pipeline(late, early);

        // Act
        await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(early.ObservedSequence, Is.LessThan(late.ObservedSequence));
    }

    [Test]
    public async Task EvaluateAsync_NewCustomHandlerRegistered_RunsWithoutModifyingExistingHandlers()
    {
        // Arrange
        RecordingModerationHandler.ResetSequence();
        StubCommentAnalyzer keyword = StubCommentAnalyzer.Clean(0.0f);
        RecordingModerationHandler custom = RecordingModerationHandler.Passing("LengthCheck", 300);
        CommentModerationPipeline pipeline = Pipeline(
            new ProfanityPreFilterHandler(keyword),
            MlHandler(StubCommentAnalyzer.Clean(0.1f), 0.5f),
            custom);

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(custom.InvocationCount, Is.EqualTo(1));
            Assert.That(keyword.InvocationCount, Is.EqualTo(1));
            Assert.That(verdict.IsFlagged, Is.False);
        });
    }

    [Test]
    public void EvaluateAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(RecordingModerationHandler.Passing("First", 100));
        using CancellationTokenSource source = new();
        source.Cancel();

        // Act
        async Task Act() => await pipeline.EvaluateAsync(CleanContext(), source.Token);

        // Assert
        Assert.That(Act, Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task Verdict_FlaggedResult_IsFlaggedEqualsLabelToxic()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(RecordingModerationHandler.Flagging("F", 100));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.That(verdict.IsFlagged, Is.EqualTo(verdict.Label == ToxicityLabel.Toxic));
    }

    [Test]
    public async Task Verdict_CleanResult_FlaggingHandlerNameIsNull()
    {
        // Arrange
        CommentModerationPipeline pipeline = Pipeline(RecordingModerationHandler.Passing("P", 100));

        // Act
        ModerationVerdict verdict = await pipeline.EvaluateAsync(CleanContext(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(verdict.IsFlagged, Is.False);
            Assert.That(verdict.FlaggingHandlerName, Is.Null);
        });
    }
}
