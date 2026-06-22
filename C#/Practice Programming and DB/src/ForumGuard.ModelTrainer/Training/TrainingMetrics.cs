namespace ForumGuard.ModelTrainer.Training;

/// <summary>
/// Captures the evaluation metrics the trainer reports to the console operator after training.
/// <para>See SDD-FORUM-024 §2.1 rule 7. Holds the multiclass <see cref="MicroAccuracy"/> and
/// <see cref="MacroAccuracy"/> together with a human-readable <see cref="ConfusionMatrix"/> rendering.</para>
/// </summary>
public sealed class TrainingMetrics
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingMetrics"/> class.
    /// </summary>
    /// <param name="microAccuracy">The micro-averaged accuracy on the validation set.</param>
    /// <param name="macroAccuracy">The macro-averaged accuracy on the validation set.</param>
    /// <param name="confusionMatrix">The formatted confusion-matrix text.</param>
    public TrainingMetrics(double microAccuracy, double macroAccuracy, string confusionMatrix)
    {
        ArgumentNullException.ThrowIfNull(confusionMatrix);
        MicroAccuracy = microAccuracy;
        MacroAccuracy = macroAccuracy;
        ConfusionMatrix = confusionMatrix;
    }

    /// <summary>
    /// Gets the micro-averaged accuracy on the validation set.
    /// </summary>
    public double MicroAccuracy { get; }

    /// <summary>
    /// Gets the macro-averaged accuracy on the validation set.
    /// </summary>
    public double MacroAccuracy { get; }

    /// <summary>
    /// Gets the formatted confusion-matrix text suitable for console reporting.
    /// </summary>
    public string ConfusionMatrix { get; }
}
