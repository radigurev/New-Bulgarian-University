using ForumGuard.Domain.Analysis;

namespace ForumGuard.Domain.Interfaces;

/// <summary>
/// Defines the Strategy contract for analyzing comment text and classifying its toxicity.
/// <para>See <see cref="AnalysisResult"/>.</para>
/// </summary>
public interface ICommentAnalyzer
{
    /// <summary>
    /// Analyzes the supplied text and produces a toxicity classification result.
    /// </summary>
    /// <param name="text">The text to analyze; may be null, empty, or whitespace.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The toxicity classification for the supplied text.</returns>
    Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default);
}
