using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Analysis;
using Microsoft.Extensions.Options;

namespace ForumGuard.Tests.Infrastructure.Analysis;

/// <summary>
/// Unit tests for <see cref="KeywordCommentAnalyzer"/> covering the SDD-FORUM-020 keyword-strategy
/// rules: profanity match, clean text, case-insensitivity, the empty-text short-circuit and the
/// missing-profanity-file configuration error.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-020")]
public sealed class KeywordCommentAnalyzerTests
{
    private string _profanityFilePath = string.Empty;

    [SetUp]
    public void SetUp()
    {
        _profanityFilePath = Path.Combine(Path.GetTempPath(), $"forumguard-profanity-{Guid.NewGuid():N}.txt");
        File.WriteAllLines(_profanityFilePath, ["# header comment", "idiot", "shut up", "moron", string.Empty]);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_profanityFilePath))
        {
            File.Delete(_profanityFilePath);
        }
    }

    [Test]
    public async Task AnalyzeAsync_TextContainingProfanityKeyword_ReturnsToxicWithScoreOne()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("you are an idiot", CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Toxic));
            Assert.That(result.Score, Is.EqualTo(1.0f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_CleanText_ReturnsCleanWithScoreZero()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("a perfectly polite remark", CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.0f));
        });
    }

    [Test]
    public async Task AnalyzeAsync_KeywordDifferentCasing_MatchesCaseInsensitively()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("What an IDIOT comment", CancellationToken.None);

        // Assert
        Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Toxic));
    }

    [Test]
    public async Task AnalyzeAsync_MultiWordKeyword_MatchesCaseInsensitively()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("Please just SHUT UP already", CancellationToken.None);

        // Assert
        Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Toxic));
    }

    [Test]
    public async Task AnalyzeAsync_NullText_ReturnsCleanWithoutMatching()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

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
    public async Task AnalyzeAsync_EmptyText_ReturnsCleanWithoutMatching()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

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
    public async Task AnalyzeAsync_WhitespaceOnlyText_ReturnsCleanWithoutMatching()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);

        // Act
        AnalysisResult result = await sut.AnalyzeAsync("   \t  \r\n ", CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Label, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(result.Score, Is.EqualTo(0.0f));
        });
    }

    [Test]
    public void AnalyzeAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        KeywordCommentAnalyzer sut = CreateAnalyzer(_profanityFilePath);
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        // Act & Assert
        Assert.That(
            async () => await sut.AnalyzeAsync("idiot", cancellationTokenSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public void Constructor_MissingProfanityFile_ThrowsAnalyzerConfigurationException()
    {
        // Arrange
        string missingPath = Path.Combine(Path.GetTempPath(), $"forumguard-missing-{Guid.NewGuid():N}.txt");
        IOptions<ModerationOptions> options = CreateOptions(missingPath);

        // Act & Assert
        Assert.That(
            () => new KeywordCommentAnalyzer(options),
            Throws.TypeOf<AnalyzerConfigurationException>());
    }

    [Test]
    public void Constructor_EmptyProfanityListPath_ThrowsAnalyzerConfigurationException()
    {
        // Arrange
        IOptions<ModerationOptions> options = CreateOptions(string.Empty);

        // Act & Assert
        Assert.That(
            () => new KeywordCommentAnalyzer(options),
            Throws.TypeOf<AnalyzerConfigurationException>());
    }

    /// <summary>
    /// Builds a <see cref="KeywordCommentAnalyzer"/> over the supplied profanity list path.
    /// </summary>
    /// <param name="profanityListPath">The profanity list file path to load keywords from.</param>
    /// <returns>A configured analyzer instance.</returns>
    private static KeywordCommentAnalyzer CreateAnalyzer(string profanityListPath)
    {
        return new KeywordCommentAnalyzer(CreateOptions(profanityListPath));
    }

    /// <summary>
    /// Wraps a <see cref="ModerationOptions"/> with the given profanity list path in an <see cref="IOptions{TOptions}"/>.
    /// </summary>
    /// <param name="profanityListPath">The profanity list file path.</param>
    /// <returns>The wrapped options.</returns>
    private static IOptions<ModerationOptions> CreateOptions(string profanityListPath)
    {
        ModerationOptions moderationOptions = new()
        {
            ProfanityListPath = profanityListPath,
            ModelPath = "unused-model-path"
        };

        return Microsoft.Extensions.Options.Options.Create(moderationOptions);
    }
}
