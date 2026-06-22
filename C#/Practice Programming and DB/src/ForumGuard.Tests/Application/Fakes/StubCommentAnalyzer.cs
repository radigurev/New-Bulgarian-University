using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// Configurable <see cref="ICommentAnalyzer"/> test double that returns a fixed result and counts invocations.
/// <para>Used to drive moderation-handler and pipeline tests without a real keyword list or ML model.</para>
/// </summary>
public sealed class StubCommentAnalyzer : ICommentAnalyzer
{
    private readonly AnalysisResult _result;

    /// <summary>
    /// Initializes the stub with the result it will return on every analysis.
    /// </summary>
    /// <param name="label">The toxicity label to return.</param>
    /// <param name="score">The toxic-score to return.</param>
    public StubCommentAnalyzer(ToxicityLabel label, float score)
    {
        _result = new AnalysisResult(label, score);
    }

    /// <summary>
    /// Gets the number of times <see cref="AnalyzeAsync"/> was invoked.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Creates a stub that always classifies text as clean with the supplied score.
    /// </summary>
    /// <param name="score">The clean-class score to return; defaults to <c>0.1</c>.</param>
    /// <returns>A clean-returning stub analyzer.</returns>
    public static StubCommentAnalyzer Clean(float score = 0.1f) =>
        new(ToxicityLabel.Clean, score);

    /// <summary>
    /// Creates a stub that always classifies text as toxic with the supplied score.
    /// </summary>
    /// <param name="score">The toxic-class score to return; defaults to <c>0.9</c>.</param>
    /// <returns>A toxic-returning stub analyzer.</returns>
    public static StubCommentAnalyzer Toxic(float score = 0.9f) =>
        new(ToxicityLabel.Toxic, score);

    /// <inheritdoc />
    public Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        InvocationCount++;
        return Task.FromResult(_result);
    }
}
