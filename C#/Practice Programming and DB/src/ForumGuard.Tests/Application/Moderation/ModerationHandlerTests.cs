using ForumGuard.Application.Moderation;
using ForumGuard.Application.Moderation.Handlers;
using ForumGuard.Application.Options;
using ForumGuard.Tests.Application.Fakes;

namespace ForumGuard.Tests.Application.Moderation;

/// <summary>
/// Verifies the two seed handlers in isolation: the profanity pre-filter flag/pass behavior and the ML handler's
/// inclusive threshold comparison read from <see cref="ModerationOptions"/> per SDD-FORUM-021 §2.5 and §3.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-021")]
public sealed class ModerationHandlerTests
{
    private static readonly CommentModerationContext Context = new("some comment text", Guid.NewGuid());

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
    public async Task ProfanityPreFilter_KeywordAnalyzerReportsToxic_ReturnsFlag()
    {
        // Arrange
        ProfanityPreFilterHandler handler = new(StubCommentAnalyzer.Toxic(1.0f));

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.True);
    }

    [Test]
    public async Task ProfanityPreFilter_KeywordAnalyzerReportsClean_ReturnsPass()
    {
        // Arrange
        ProfanityPreFilterHandler handler = new(StubCommentAnalyzer.Clean(0.0f));

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.False);
    }

    [Test]
    public void ProfanityPreFilter_Order_RunsBeforeMlToxicity()
    {
        // Arrange
        ProfanityPreFilterHandler profanity = new(StubCommentAnalyzer.Clean(0.0f));
        MlToxicityHandler ml = MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f);

        // Act
        // Assert
        Assert.That(profanity.Order, Is.LessThan(ml.Order));
    }

    [Test]
    public async Task MlToxicity_ScoreAboveThreshold_ReturnsFlag()
    {
        // Arrange
        MlToxicityHandler handler = MlHandler(StubCommentAnalyzer.Toxic(0.7f), 0.5f);

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.True);
    }

    [Test]
    public async Task MlToxicity_ScoreEqualToThreshold_ReturnsFlagInclusively()
    {
        // Arrange
        MlToxicityHandler handler = MlHandler(StubCommentAnalyzer.Toxic(0.5f), 0.5f);

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.True);
    }

    [Test]
    public async Task MlToxicity_ScoreBelowThreshold_ReturnsPass()
    {
        // Arrange
        MlToxicityHandler handler = MlHandler(StubCommentAnalyzer.Clean(0.49f), 0.5f);

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.False);
    }

    [Test]
    public async Task MlToxicity_CustomThresholdFromOptions_AppliesConfiguredCutoff()
    {
        // Arrange
        MlToxicityHandler handler = MlHandler(StubCommentAnalyzer.Clean(0.6f), 0.8f);

        // Act
        HandlerResult result = await handler.EvaluateAsync(Context, CancellationToken.None);

        // Assert
        Assert.That(result.IsFlag, Is.False);
    }

    [Test]
    public void ProfanityPreFilter_Name_IsHandlerTypeName()
    {
        // Arrange
        ProfanityPreFilterHandler handler = new(StubCommentAnalyzer.Clean(0.0f));

        // Act
        // Assert
        Assert.That(handler.Name, Is.EqualTo(nameof(ProfanityPreFilterHandler)));
    }

    [Test]
    public void MlToxicity_Name_IsHandlerTypeName()
    {
        // Arrange
        MlToxicityHandler handler = MlHandler(StubCommentAnalyzer.Clean(0.0f), 0.5f);

        // Act
        // Assert
        Assert.That(handler.Name, Is.EqualTo(nameof(MlToxicityHandler)));
    }
}
