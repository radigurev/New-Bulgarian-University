using ForumGuard.Domain.Enums;

namespace ForumGuard.Domain.Analysis;

/// <summary>
/// Represents the outcome of analyzing a comment's text for toxicity.
/// <para>The <see cref="Score"/> is the toxic-class probability in the inclusive range <c>[0.0, 1.0]</c>.</para>
/// <para>See <see cref="ToxicityLabel"/>.</para>
/// </summary>
public sealed record AnalysisResult
{
    /// <summary>
    /// Initializes a new analysis result, validating that <paramref name="score"/> is within <c>[0.0, 1.0]</c>.
    /// </summary>
    /// <param name="label">The classification assigned to the analyzed text.</param>
    /// <param name="score">The toxic-class probability in the inclusive range <c>[0.0, 1.0]</c>.</param>
    public AnalysisResult(ToxicityLabel label, float score)
    {
        if (float.IsNaN(score) || float.IsInfinity(score) || score < 0.0f || score > 1.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(score), score, "Score must be a finite value within the inclusive range [0.0, 1.0].");
        }

        Label = label;
        Score = score;
    }

    /// <summary>
    /// Gets the classification assigned to the analyzed text.
    /// </summary>
    public ToxicityLabel Label { get; }

    /// <summary>
    /// Gets the toxic-class probability in the inclusive range <c>[0.0, 1.0]</c>.
    /// </summary>
    public float Score { get; }
}
