using ForumGuard.Domain.Enums;

namespace ForumGuard.ModelTrainer.Dataset;

/// <summary>
/// Provides the authoritative label-string constants for the seed corpus and the trained model.
/// <para>See SDD-FORUM-024 §2.2 and SDD-FORUM-010. The values mirror the <see cref="ToxicityLabel"/>
/// members so the CSV <c>Label</c> column and the trained <c>PredictedLabel</c> share one vocabulary.</para>
/// </summary>
public static class TrainingLabels
{
    /// <summary>
    /// The non-toxic label string, matching <see cref="ToxicityLabel.Clean"/>.
    /// </summary>
    public static readonly string Clean = ToxicityLabel.Clean.ToString();

    /// <summary>
    /// The toxic label string, matching <see cref="ToxicityLabel.Toxic"/>.
    /// </summary>
    public static readonly string Toxic = ToxicityLabel.Toxic.ToString();

    /// <summary>
    /// Determines whether the supplied label string is one of the two permitted labels.
    /// </summary>
    /// <param name="label">The candidate label string.</param>
    /// <returns><see langword="true"/> when the label is exactly <c>Clean</c> or <c>Toxic</c>.</returns>
    public static bool IsValid(string label)
    {
        return string.Equals(label, Clean, StringComparison.Ordinal)
            || string.Equals(label, Toxic, StringComparison.Ordinal);
    }
}
