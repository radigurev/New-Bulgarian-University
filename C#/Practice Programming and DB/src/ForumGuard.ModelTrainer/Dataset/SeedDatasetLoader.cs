using System.Text;
using ForumGuard.ModelTrainer.Csv;
using ForumGuard.ModelTrainer.Exceptions;

namespace ForumGuard.ModelTrainer.Dataset;

/// <summary>
/// Loads and validates the hand-authored <c>Text,Label</c> seed corpus into <see cref="TrainingDataRow"/>
/// instances, enforcing the SDD-FORUM-024 dataset contract.
/// <para>Validates that the file exists and is readable (E1), the header is exactly <c>Text,Label</c>
/// (E4), every row parses to two RFC-4180 fields with a label of <c>Clean</c> or <c>Toxic</c> (E4),
/// both classes are present (E2 / DV-1), and the minimum row counts are met (E3 / DV-2). The file is
/// read as UTF-8.</para>
/// </summary>
public sealed class SeedDatasetLoader
{
    private static readonly string[] RequiredHeader = ["Text", "Label"];

    private readonly DatasetValidationOptions _validationOptions;

    /// <summary>
    /// Initializes the loader with the default validation thresholds.
    /// </summary>
    public SeedDatasetLoader()
        : this(DatasetValidationOptions.Default)
    {
    }

    /// <summary>
    /// Initializes the loader with the supplied validation thresholds.
    /// </summary>
    /// <param name="validationOptions">The minimum-row thresholds to enforce.</param>
    public SeedDatasetLoader(DatasetValidationOptions validationOptions)
    {
        ArgumentNullException.ThrowIfNull(validationOptions);
        _validationOptions = validationOptions;
    }

    /// <summary>
    /// Loads and validates the seed corpus at the supplied path.
    /// </summary>
    /// <param name="datasetPath">The path to the seed CSV file.</param>
    /// <returns>The validated training rows.</returns>
    /// <exception cref="DatasetNotFoundException">Thrown when the path is empty, missing, or unreadable (E1).</exception>
    /// <exception cref="InvalidDatasetException">Thrown when the corpus violates the dataset contract (E2/E3/E4).</exception>
    public IReadOnlyList<TrainingDataRow> Load(string datasetPath)
    {
        string content = ReadContent(datasetPath);
        IReadOnlyList<IReadOnlyList<string>> records = Rfc4180Reader.Parse(content);

        ValidateHeader(records);
        IReadOnlyList<TrainingDataRow> rows = ParseDataRows(records);
        ValidateRowCounts(rows);

        return rows;
    }

    private static string ReadContent(string datasetPath)
    {
        if (string.IsNullOrWhiteSpace(datasetPath))
        {
            throw new DatasetNotFoundException("The training dataset path is empty; a readable CSV path is required.");
        }

        if (!File.Exists(datasetPath))
        {
            throw new DatasetNotFoundException($"The training dataset was not found or is unreadable at '{datasetPath}'.");
        }

        try
        {
            return File.ReadAllText(datasetPath, Encoding.UTF8);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new DatasetNotFoundException($"The training dataset at '{datasetPath}' could not be read.", exception);
        }
    }

    private static void ValidateHeader(IReadOnlyList<IReadOnlyList<string>> records)
    {
        if (records.Count == 0)
        {
            throw new InvalidDatasetException("Dataset format error: the file is empty; a 'Text,Label' header row is required.");
        }

        IReadOnlyList<string> header = records[0];
        bool headerMatches = header.Count == RequiredHeader.Length
            && string.Equals(header[0], RequiredHeader[0], StringComparison.Ordinal)
            && string.Equals(header[1], RequiredHeader[1], StringComparison.Ordinal);

        if (!headerMatches)
        {
            throw new InvalidDatasetException(
                $"Dataset format error: the header row must be exactly 'Text,Label' but was '{string.Join(',', header)}'.");
        }
    }

    private IReadOnlyList<TrainingDataRow> ParseDataRows(IReadOnlyList<IReadOnlyList<string>> records)
    {
        List<TrainingDataRow> rows = [];
        for (int recordIndex = 1; recordIndex < records.Count; recordIndex++)
        {
            IReadOnlyList<string> record = records[recordIndex];
            if (IsBlankRecord(record))
            {
                continue;
            }

            rows.Add(ParseRow(record, recordIndex + 1));
        }

        return rows;
    }

    private static bool IsBlankRecord(IReadOnlyList<string> record)
    {
        return record.Count == 1 && string.IsNullOrWhiteSpace(record[0]);
    }

    private static TrainingDataRow ParseRow(IReadOnlyList<string> record, int lineNumber)
    {
        if (record.Count != 2)
        {
            throw new InvalidDatasetException(
                $"Dataset format error on line {lineNumber}: expected exactly 2 fields (Text,Label) but found {record.Count}.");
        }

        string text = record[0];
        string label = record[1].Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidDatasetException(
                $"Dataset format error on line {lineNumber}: the Text field must be non-empty and non-whitespace.");
        }

        if (!TrainingLabels.IsValid(label))
        {
            throw new InvalidDatasetException(
                $"Dataset format error on line {lineNumber}: Label '{label}' is neither '{TrainingLabels.Clean}' nor '{TrainingLabels.Toxic}'.");
        }

        return new TrainingDataRow { Text = text, Label = label };
    }

    private void ValidateRowCounts(IReadOnlyList<TrainingDataRow> rows)
    {
        int cleanCount = rows.Count(row => string.Equals(row.Label, TrainingLabels.Clean, StringComparison.Ordinal));
        int toxicCount = rows.Count - cleanCount;

        if (cleanCount == 0 || toxicCount == 0)
        {
            throw new InvalidDatasetException(
                $"Single-class dataset: both '{TrainingLabels.Clean}' and '{TrainingLabels.Toxic}' rows are required "
                + $"(found Clean={cleanCount}, Toxic={toxicCount}).");
        }

        if (rows.Count < _validationOptions.MinimumTotalRows
            || cleanCount < _validationOptions.MinimumRowsPerClass
            || toxicCount < _validationOptions.MinimumRowsPerClass)
        {
            throw new InvalidDatasetException(
                $"Insufficient training data: require at least {_validationOptions.MinimumTotalRows} rows total and "
                + $"{_validationOptions.MinimumRowsPerClass} per class, but found {rows.Count} total "
                + $"(Clean={cleanCount}, Toxic={toxicCount}).");
        }
    }
}
