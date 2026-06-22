using ForumGuard.ModelTrainer.Dataset;
using ForumGuard.ModelTrainer.Exceptions;
using ForumGuard.ModelTrainer.Training;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ForumGuard.ModelTrainer.Cli;

/// <summary>
/// Orchestrates a single offline trainer run: parse arguments, load and validate the seed corpus,
/// split, train, evaluate, report metrics, and export <c>model.zip</c>.
/// <para>See SDD-FORUM-024. Each failure mode maps to a distinct non-zero <see cref="TrainerExitCode"/>
/// and produces no artifact; a missing native <c>libtorch-cpu</c> backend at training time is detected
/// and reported with a clear operator note rather than an opaque stack trace.</para>
/// </summary>
public sealed class TrainerRunner
{
    private readonly TextWriter _output;
    private readonly TextWriter _error;

    /// <summary>
    /// Initializes the runner with the console output and error writers.
    /// </summary>
    /// <param name="output">The writer for operator-facing progress and metrics.</param>
    /// <param name="error">The writer for operator-facing error messages.</param>
    public TrainerRunner(TextWriter output, TextWriter error)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);
        _output = output;
        _error = error;
    }

    /// <summary>
    /// Executes the trainer run for the supplied command-line arguments.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    /// <param name="cancellationToken">A token observed for operator cancellation.</param>
    /// <returns>The process exit code; <see cref="TrainerExitCode.Success"/> on a completed export.</returns>
    public int Run(IReadOnlyList<string> args, CancellationToken cancellationToken)
    {
        try
        {
            TrainerArguments arguments = TrainerArgumentParser.Parse(args);
            return Execute(arguments, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return Fail(TrainerExitCode.InvalidArguments, "Invalid arguments", exception.Message);
        }
        catch (DatasetNotFoundException exception)
        {
            return Fail(TrainerExitCode.DatasetNotFound, "Training dataset not found / unreadable", exception.Message);
        }
        catch (InvalidDatasetException exception)
        {
            return Fail(TrainerExitCode.InvalidDataset, "Dataset validation failed", exception.Message);
        }
        catch (ModelExportException exception)
        {
            return Fail(TrainerExitCode.ExportFailed, "Model export failed", exception.Message);
        }
        catch (OperationCanceledException)
        {
            return Fail(TrainerExitCode.Cancelled, "Training cancelled", "The operator cancelled the run; no model.zip was produced.");
        }
        catch (Exception exception)
        {
            return FailUnexpected(exception);
        }
    }

    private int Execute(TrainerArguments arguments, CancellationToken cancellationToken)
    {
        SeedDatasetLoader loader = new();
        IReadOnlyList<TrainingDataRow> rows = loader.Load(arguments.DatasetPath);
        _output.WriteLine($"Loaded and validated {rows.Count} rows from '{arguments.DatasetPath}'.");

        cancellationToken.ThrowIfCancellationRequested();

        MLContext mlContext = new(seed: 1);
        IDataView dataView = mlContext.Data.LoadFromEnumerable(rows);
        DataOperationsCatalog.TrainTestData split = mlContext.Data.TrainTestSplit(
            dataView, testFraction: arguments.ValidationFraction, seed: 1);

        _output.WriteLine($"Training NAS-BERT TextClassification model (maxEpochs={arguments.Epochs}, slow on CPU)...");
        TrainingPipeline pipeline = new(mlContext);
        ITransformer trainedModel = pipeline.Train(split.TrainSet, split.TestSet, arguments.Epochs);

        ReportMetrics(pipeline.Evaluate(trainedModel, split.TestSet));

        cancellationToken.ThrowIfCancellationRequested();

        ModelExporter exporter = new(mlContext);
        exporter.Export(trainedModel, dataView.Schema, arguments.OutputPath);
        _output.WriteLine($"Exported model.zip to '{arguments.OutputPath}'.");

        return TrainerExitCode.Success;
    }

    private void ReportMetrics(TrainingMetrics metrics)
    {
        _output.WriteLine($"MicroAccuracy: {metrics.MicroAccuracy:F4}");
        _output.WriteLine($"MacroAccuracy: {metrics.MacroAccuracy:F4}");
        _output.WriteLine("Confusion matrix:");
        _output.WriteLine(metrics.ConfusionMatrix);
    }

    private int Fail(int exitCode, string heading, string detail)
    {
        _error.WriteLine($"ABORT: {heading}.");
        _error.WriteLine(detail);
        return exitCode;
    }

    private int FailUnexpected(Exception exception)
    {
        if (LooksLikeMissingLibtorch(exception))
        {
            _error.WriteLine("ABORT: native libtorch backend not found.");
            _error.WriteLine(
                "Training requires the native 'libtorch-cpu' package (a large download). Add it to "
                + "ForumGuard.ModelTrainer and re-run. See src/ForumGuard.ModelTrainer/README.md.");
            return TrainerExitCode.UnexpectedFailure;
        }

        _error.WriteLine("ABORT: unexpected training failure.");
        _error.WriteLine(exception.ToString());
        return TrainerExitCode.UnexpectedFailure;
    }

    private static bool LooksLikeMissingLibtorch(Exception exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            string message = current.Message;
            bool mentionsTorch = message.Contains("libtorch", StringComparison.OrdinalIgnoreCase)
                || message.Contains("torch_cpu", StringComparison.OrdinalIgnoreCase)
                || message.Contains("LibTorchSharp", StringComparison.OrdinalIgnoreCase);
            if (current is DllNotFoundException || (current is TypeInitializationException && mentionsTorch) || mentionsTorch)
            {
                return true;
            }
        }

        return false;
    }
}
