using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Analysis;
using ForumGuard.Tests.Application.Fakes;
using ForumGuard.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

namespace ForumGuard.Tests.Infrastructure.Analysis;

/// <summary>
/// Unit tests for <see cref="NasBertCommentAnalyzer"/> covering the SDD-FORUM-020 decision rules:
/// the inclusive <c>Score &gt;= ToxicityThreshold</c> labelling, the toxic-score mapping, the
/// empty-text short-circuit, the operator-tunable threshold, the fail-safe
/// <see cref="AnalyzerUnavailableException"/> on inference failure or an empty score vector, and
/// cooperative cancellation.
/// <para>The analyzer is driven through a real <see cref="PredictionEnginePool{ModelInput, ModelOutput}"/>
/// loaded from a tiny purely managed model (<see cref="ManagedToxicityModelFixture"/>). No native
/// <c>libtorch-cpu</c> backend and no NAS-BERT training are involved — the model is a deterministic
/// managed transform whose output is encoded in the input text.</para>
/// </summary>
[TestFixture]
[Category("SDD-FORUM-020")]
public sealed class NasBertCommentAnalyzerTests
{
    private string _modelPath = string.Empty;
    private ServiceProvider _provider = null!;
    private PredictionEnginePool<ModelInput, ModelOutput> _pool = null!;

    [SetUp]
    public void SetUp()
    {
        _modelPath = ManagedToxicityModelFixture.CreateModelFile();
        _provider = PredictionEnginePoolFactory.BuildProvider(_modelPath, ManagedToxicityModelFixture.ModelName);
        _pool = PredictionEnginePoolFactory.ResolvePool(_provider);
    }

    [TearDown]
    public void TearDown()
    {
        _provider?.Dispose();
        if (File.Exists(_modelPath))
        {
            File.Delete(_modelPath);
        }
    }

    [Test]
    public async Task AnalyzeAsync_ToxicScoreEqualToThreshold_ReturnsToxic()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);
        string text = ManagedToxicityModelFixture.EncodeToxicScore(0.5f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(text, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(result.Score, Is.EqualTo(0.5f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_ToxicScoreAboveThreshold_ReturnsToxic()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);
        string text = ManagedToxicityModelFixture.EncodeToxicScore(0.9f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(text, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(result.Score, Is.EqualTo(0.9f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_ToxicScoreBelowThreshold_ReturnsClean()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);
        string text = ManagedToxicityModelFixture.EncodeToxicScore(0.2f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(text, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.2f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_ModelOutputScore_MapsToxicProbabilityIntoAnalysisResultScore()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);
        string text = ManagedToxicityModelFixture.EncodeToxicScore(0.73f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(text, CancellationToken.None);

        // Assert
        Assert.That(result.Score, Is.EqualTo(0.73f).Within(0.0001f));
    }

    [Test]
    public async Task AnalyzeAsync_CustomThresholdFromModerationOptions_AppliesConfiguredCutoff()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.8f);
        string belowText = ManagedToxicityModelFixture.EncodeToxicScore(0.7f);
        string atText = ManagedToxicityModelFixture.EncodeToxicScore(0.8f);

        // Act
        AnalysisResult below = await sut.AnalyzeAsync(belowText, CancellationToken.None);
        AnalysisResult atOrAbove = await sut.AnalyzeAsync(atText, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(below.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(atOrAbove.Label, Is.EqualTo(ToxicityLabel.Toxic));
        });
    }

    [Test]
    public async Task AnalyzeAsync_NullText_ReturnsCleanWithoutInvokingPool()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(null!, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.0f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_EmptyText_ReturnsCleanWithoutInvokingPool()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync(string.Empty, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.0f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_WhitespaceOnlyText_ReturnsCleanWithoutInvokingPool()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("   \t \r\n ", CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.0f));
        });
    }

    [Test]
    public void AnalyzeAsync_PoolPredictThrows_WrapsInAnalyzerUnavailableExceptionNotClean()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);

        // Act & Assert
        Assert.That(
            async () => await sut.AnalyzeAsync(ManagedToxicityModelFixture.ThrowingText, CancellationToken.None),
            Throws.TypeOf<AnalyzerUnavailableException>());
    }

    [Test]
    public void AnalyzeAsync_ModelReturnsEmptyScoreVector_ThrowsAnalyzerUnavailableExceptionNotClean()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);

        // Act & Assert
        Assert.That(
            async () => await sut.AnalyzeAsync(ManagedToxicityModelFixture.EmptyScoreText, CancellationToken.None),
            Throws.TypeOf<AnalyzerUnavailableException>());
    }

    [Test]
    public void AnalyzeAsync_CancellationRequested_PropagatesOperationCanceledException()
    {
        // Arrange
        NasBertCommentAnalyzer sut = CreateAnalyzer(threshold: 0.5f);
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();
        string text = ManagedToxicityModelFixture.EncodeToxicScore(0.9f);

        // Act & Assert
        Assert.That(
            async () => await sut.AnalyzeAsync(text, cancellationTokenSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void Constructor_NullPool_ThrowsArgumentNullException()
    {
        // Arrange
        IOptionsMonitor<ModerationOptions> options = CreateOptions(0.5f);

        // Act & Assert
        Assert.That(
            () => new NasBertCommentAnalyzer(null!, options, ManagedToxicityModelFixture.ModelName),
            Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Constructor_EmptyModelName_ThrowsArgumentException()
    {
        // Arrange
        IOptionsMonitor<ModerationOptions> options = CreateOptions(0.5f);

        // Act & Assert
        Assert.That(
            () => new NasBertCommentAnalyzer(_pool, options, string.Empty),
            Throws.InstanceOf<ArgumentException>());
    }

    /// <summary>
    /// Builds a <see cref="NasBertCommentAnalyzer"/> over the managed test pool with the supplied threshold.
    /// </summary>
    /// <param name="threshold">The toxicity threshold to apply.</param>
    /// <returns>The analyzer under test.</returns>
    private NasBertCommentAnalyzer CreateAnalyzer(float threshold)
    {
        return new NasBertCommentAnalyzer(_pool, CreateOptions(threshold), ManagedToxicityModelFixture.ModelName);
    }

    /// <summary>
    /// Builds an options monitor exposing a <see cref="ModerationOptions"/> with the supplied threshold.
    /// </summary>
    /// <param name="threshold">The toxicity threshold to expose.</param>
    /// <returns>The options monitor.</returns>
    private static IOptionsMonitor<ModerationOptions> CreateOptions(float threshold)
    {
        ModerationOptions options = new()
        {
            ToxicityThreshold = threshold,
            ModelPath = "unused-model-path",
            ProfanityListPath = "unused-profanity-path"
        };

        return new StaticOptionsMonitor<ModerationOptions>(options);
    }
}
