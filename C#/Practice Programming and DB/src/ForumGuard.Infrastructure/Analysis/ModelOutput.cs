namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Represents the output schema produced by the NAS-BERT TextClassification model at inference time.
/// <para>See SDD-FORUM-020 and SDD-FORUM-024. <see cref="PredictedLabel"/> is the model's chosen class
/// string (<c>"Clean"</c> or <c>"Toxic"</c>); <see cref="Score"/> is the per-class probability vector
/// aligned to the trained classes. <see cref="NasBertCommentAnalyzer"/> derives the
/// <c>AnalysisResult.Label</c> from the toxic-class probability versus the configured threshold rather
/// than trusting <see cref="PredictedLabel"/>.</para>
/// </summary>
public sealed class ModelOutput
{
    /// <summary>
    /// Gets or sets the class string the model predicted; one of the trained labels (<c>"Clean"</c> / <c>"Toxic"</c>).
    /// </summary>
    public string PredictedLabel { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the per-class probability vector aligned to the trained classes; non-empty after a successful prediction.
    /// </summary>
    public float[] Score { get; set; } = [];
}
