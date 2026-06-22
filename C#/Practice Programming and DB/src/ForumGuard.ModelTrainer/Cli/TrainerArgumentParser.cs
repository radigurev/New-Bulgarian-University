namespace ForumGuard.ModelTrainer.Cli;

/// <summary>
/// Parses the trainer's command-line arguments (<c>--data</c>, <c>--output</c>, <c>--validation-fraction</c>,
/// <c>--epochs</c>), applying the SDD-FORUM-024 defaults.
/// <para>See SDD-FORUM-024 §2.1 rule 2. The dataset defaults to <c>seed-comments.csv</c> resolved
/// against the application base directory; the output defaults to <c>model.zip</c>; the validation
/// fraction defaults to <c>0.2</c>; the epochs default to <c>10</c>.</para>
/// </summary>
public static class TrainerArgumentParser
{
    private const string DataFlag = "--data";
    private const string OutputFlag = "--output";
    private const string ValidationFractionFlag = "--validation-fraction";
    private const string EpochsFlag = "--epochs";
    private const string DefaultDatasetFileName = "seed-comments.csv";
    private const string DefaultOutputFileName = "model.zip";
    private const double DefaultValidationFraction = 0.2;
    private const int DefaultEpochs = 10;

    /// <summary>
    /// Parses the supplied arguments into a <see cref="TrainerArguments"/> using SDD-FORUM-024 defaults.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <returns>The parsed arguments.</returns>
    /// <exception cref="ArgumentException">Thrown when a flag is supplied without a value or the fraction is unparseable.</exception>
    public static TrainerArguments Parse(IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);

        string datasetPath = Path.Combine(AppContext.BaseDirectory, DefaultDatasetFileName);
        string outputPath = Path.Combine(AppContext.BaseDirectory, DefaultOutputFileName);
        double validationFraction = DefaultValidationFraction;
        int epochs = DefaultEpochs;

        for (int i = 0; i < args.Count; i++)
        {
            string flag = args[i];
            switch (flag)
            {
                case DataFlag:
                    datasetPath = RequireValue(args, ref i, DataFlag);
                    break;
                case OutputFlag:
                    outputPath = RequireValue(args, ref i, OutputFlag);
                    break;
                case ValidationFractionFlag:
                    validationFraction = ParseFraction(RequireValue(args, ref i, ValidationFractionFlag));
                    break;
                case EpochsFlag:
                    epochs = ParseEpochs(RequireValue(args, ref i, EpochsFlag));
                    break;
                default:
                    throw new ArgumentException($"Unknown argument '{flag}'.", nameof(args));
            }
        }

        return new TrainerArguments(datasetPath, outputPath, validationFraction, epochs);
    }

    private static string RequireValue(IReadOnlyList<string> args, ref int index, string flag)
    {
        if (index + 1 >= args.Count)
        {
            throw new ArgumentException($"The '{flag}' argument requires a value.", nameof(args));
        }

        index++;
        return args[index];
    }

    private static double ParseFraction(string value)
    {
        if (!double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double fraction))
        {
            throw new ArgumentException($"The validation fraction '{value}' is not a valid number.", nameof(value));
        }

        return fraction;
    }

    private static int ParseEpochs(string value)
    {
        if (!int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int epochs))
        {
            throw new ArgumentException($"The epochs value '{value}' is not a valid integer.", nameof(value));
        }

        if (epochs < 1)
        {
            throw new ArgumentException($"The epochs value '{value}' must be at least 1.", nameof(value));
        }

        return epochs;
    }
}
