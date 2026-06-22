using ForumGuard.ModelTrainer.Exceptions;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ForumGuard.ModelTrainer.Training;

/// <summary>
/// Exports a trained transformer to a <c>model.zip</c> artifact loadable by
/// <c>PredictionEnginePool&lt;ModelInput, ModelOutput&gt;.FromFile</c> (SDD-FORUM-020).
/// <para>See SDD-FORUM-024 §2.1 rule 8 and E5. The model is first written to a temporary file in the
/// destination directory and then atomically moved into place, so a failed save never leaves a
/// partial/corrupt artifact over an existing good one. Any IO or ML.NET save failure is surfaced as a
/// <see cref="ModelExportException"/>.</para>
/// </summary>
public sealed class ModelExporter
{
    private readonly MLContext _mlContext;

    /// <summary>
    /// Initializes the exporter with the supplied ML.NET context.
    /// </summary>
    /// <param name="mlContext">The ML.NET context used to save the model.</param>
    public ModelExporter(MLContext mlContext)
    {
        ArgumentNullException.ThrowIfNull(mlContext);
        _mlContext = mlContext;
    }

    /// <summary>
    /// Saves the trained model to the output path, preserving the input schema so the artifact is
    /// loadable by the runtime prediction-engine pool.
    /// </summary>
    /// <param name="trainedModel">The trained transformer to save.</param>
    /// <param name="inputSchema">The training data input schema to persist with the model.</param>
    /// <param name="outputPath">The destination <c>model.zip</c> path.</param>
    /// <exception cref="ModelExportException">Thrown when the destination is not writable or the save fails (E5).</exception>
    public void Export(ITransformer trainedModel, DataViewSchema inputSchema, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(trainedModel);
        ArgumentNullException.ThrowIfNull(inputSchema);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        EnsureDirectoryExists(outputPath);
        string temporaryPath = outputPath + ".tmp";

        try
        {
            _mlContext.Model.Save(trainedModel, inputSchema, temporaryPath);
            File.Move(temporaryPath, outputPath, overwrite: true);
        }
        catch (Exception exception)
        {
            CleanupTemporary(temporaryPath);
            throw new ModelExportException(
                $"The trained model could not be exported to '{outputPath}'; no artifact was written.", exception);
        }
    }

    private static void EnsureDirectoryExists(string outputPath)
    {
        string? directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>
    /// Performs a best-effort deletion of the temporary file after a failed save; the original export
    /// failure remains the reported error so cleanup faults are intentionally suppressed.
    /// </summary>
    /// <param name="temporaryPath">The temporary file to remove.</param>
    private static void CleanupTemporary(string temporaryPath)
    {
        try
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
        catch (Exception)
        {
            return;
        }
    }
}
