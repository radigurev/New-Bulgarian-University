namespace ForumGuard.ModelTrainer.Cli;

/// <summary>
/// Carries the parsed command-line arguments for a trainer run.
/// <para>See SDD-FORUM-024 §2.1 rule 2: the dataset defaults to <c>seed-comments.csv</c> and the
/// output to <c>model.zip</c>; the output SHOULD be the operator's <c>Moderation:ModelPath</c> for a
/// hot-reload deployment.</para>
/// </summary>
public sealed class TrainerArguments
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TrainerArguments"/> class.
    /// </summary>
    /// <param name="datasetPath">The resolved seed-dataset path.</param>
    /// <param name="outputPath">The resolved output <c>model.zip</c> path.</param>
    /// <param name="validationFraction">The fraction of data held out for validation.</param>
    /// <param name="epochs">The NAS-BERT training length; must be at least 1. Defaults to 10.</param>
    public TrainerArguments(string datasetPath, string outputPath, double validationFraction, int epochs = 10)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(datasetPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        if (validationFraction is <= 0.0 or >= 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(validationFraction), validationFraction, "Validation fraction must be within the exclusive range (0.0, 1.0).");
        }

        if (epochs < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(epochs), epochs, "Epochs must be at least 1.");
        }

        DatasetPath = datasetPath;
        OutputPath = outputPath;
        ValidationFraction = validationFraction;
        Epochs = epochs;
    }

    /// <summary>
    /// Gets the seed-dataset path.
    /// </summary>
    public string DatasetPath { get; }

    /// <summary>
    /// Gets the output <c>model.zip</c> path.
    /// </summary>
    public string OutputPath { get; }

    /// <summary>
    /// Gets the fraction of data held out for validation.
    /// </summary>
    public double ValidationFraction { get; }

    /// <summary>
    /// Gets the NAS-BERT training length in epochs.
    /// </summary>
    public int Epochs { get; }
}
