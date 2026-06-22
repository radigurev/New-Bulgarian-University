namespace ForumGuard.ModelTrainer.Dataset;

/// <summary>
/// Carries the configurable minimum-row thresholds the <see cref="SeedDatasetLoader"/> enforces.
/// <para>See SDD-FORUM-024 §2.2 rule 13 and DV-2: a default minimum of <c>10</c> rows overall with at
/// least <c>2</c> rows per class.</para>
/// </summary>
public sealed class DatasetValidationOptions
{
    /// <summary>
    /// Gets the default validation options (minimum total <c>10</c>, minimum per class <c>2</c>).
    /// </summary>
    public static DatasetValidationOptions Default { get; } = new(minimumTotalRows: 10, minimumRowsPerClass: 2);

    /// <summary>
    /// Initializes a new instance of the <see cref="DatasetValidationOptions"/> class.
    /// </summary>
    /// <param name="minimumTotalRows">The minimum total number of data rows required.</param>
    /// <param name="minimumRowsPerClass">The minimum number of rows required for each class.</param>
    public DatasetValidationOptions(int minimumTotalRows, int minimumRowsPerClass)
    {
        if (minimumTotalRows < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumTotalRows), minimumTotalRows, "Minimum total rows must be positive.");
        }

        if (minimumRowsPerClass < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumRowsPerClass), minimumRowsPerClass, "Minimum rows per class must be positive.");
        }

        MinimumTotalRows = minimumTotalRows;
        MinimumRowsPerClass = minimumRowsPerClass;
    }

    /// <summary>
    /// Gets the minimum total number of data rows the corpus must contain.
    /// </summary>
    public int MinimumTotalRows { get; }

    /// <summary>
    /// Gets the minimum number of rows each class must contain.
    /// </summary>
    public int MinimumRowsPerClass { get; }
}
