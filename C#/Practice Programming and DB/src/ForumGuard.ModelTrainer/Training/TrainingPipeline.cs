using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;

namespace ForumGuard.ModelTrainer.Training;

/// <summary>
/// Builds the NAS-BERT TextClassification training pipeline, trains it on a data split, and evaluates it.
/// <para>See SDD-FORUM-024 §2.1 rules 4-7 and SDD-FORUM-020 §2.3. The estimator chain is
/// <c>MapValueToKey("Label") → MulticlassClassification.Trainers.TextClassification(labelColumnName:
/// "Label", sentence1ColumnName: "Text") → MapKeyToValue("PredictedLabel")</c>. The estimator is built
/// without requiring the native <c>libtorch-cpu</c> backend; the backend is only needed when
/// <see cref="Train(IDataView, IDataView, int)"/> calls <c>Fit</c> at the operator's runtime.</para>
/// </summary>
public sealed class TrainingPipeline
{
    /// <summary>
    /// The output column name carrying the predicted label string after key-to-value mapping.
    /// </summary>
    public const string PredictedLabelColumnName = "PredictedLabel";

    /// <summary>
    /// The label column name shared by the seed CSV and the trainer.
    /// </summary>
    public const string LabelColumnName = "Label";

    /// <summary>
    /// The text column name mapped onto the NAS-BERT <c>sentence1ColumnName</c>.
    /// </summary>
    public const string TextColumnName = "Text";

    /// <summary>
    /// The default NAS-BERT training length, matching the trainer's historical behaviour.
    /// </summary>
    public const int DefaultMaxEpochs = 10;

    private readonly MLContext _mlContext;

    /// <summary>
    /// Initializes the pipeline with the supplied ML.NET context.
    /// </summary>
    /// <param name="mlContext">The ML.NET context used to build, train, and evaluate the pipeline.</param>
    public TrainingPipeline(MLContext mlContext)
    {
        ArgumentNullException.ThrowIfNull(mlContext);
        _mlContext = mlContext;
    }

    /// <summary>
    /// Builds the TextClassification estimator chain without training it.
    /// </summary>
    /// <param name="maxEpochs">The NAS-BERT training length; defaults to <see cref="DefaultMaxEpochs"/>.</param>
    /// <returns>The composed estimator: map-label-to-key → TextClassification → map-key-to-predicted-label.</returns>
    public IEstimator<ITransformer> BuildPipeline(int maxEpochs = DefaultMaxEpochs)
    {
        return _mlContext.Transforms.Conversion
            .MapValueToKey(outputColumnName: LabelColumnName, inputColumnName: LabelColumnName)
            .Append(_mlContext.MulticlassClassification.Trainers.TextClassification(
                labelColumnName: LabelColumnName,
                sentence1ColumnName: TextColumnName,
                maxEpochs: maxEpochs))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue(
                outputColumnName: PredictedLabelColumnName,
                inputColumnName: PredictedLabelColumnName));
    }

    /// <summary>
    /// Trains the pipeline on the supplied training split. Requires the native <c>libtorch-cpu</c>
    /// backend at runtime; never invoked during a build.
    /// </summary>
    /// <param name="trainingData">The training split.</param>
    /// <param name="validationData">The validation split, evaluated by <see cref="Evaluate"/>.</param>
    /// <param name="maxEpochs">The NAS-BERT training length; defaults to <see cref="DefaultMaxEpochs"/>.</param>
    /// <returns>The trained transformer.</returns>
    public ITransformer Train(IDataView trainingData, IDataView validationData, int maxEpochs = DefaultMaxEpochs)
    {
        ArgumentNullException.ThrowIfNull(trainingData);
        ArgumentNullException.ThrowIfNull(validationData);

        IEstimator<ITransformer> pipeline = BuildPipeline(maxEpochs);
        return pipeline.Fit(trainingData);
    }

    /// <summary>
    /// Evaluates a trained transformer on the validation split and captures the reporting metrics.
    /// </summary>
    /// <param name="trainedModel">The trained transformer.</param>
    /// <param name="validationData">The validation split.</param>
    /// <returns>The captured micro/macro accuracy and confusion-matrix text.</returns>
    public TrainingMetrics Evaluate(ITransformer trainedModel, IDataView validationData)
    {
        ArgumentNullException.ThrowIfNull(trainedModel);
        ArgumentNullException.ThrowIfNull(validationData);

        IDataView predictions = trainedModel.Transform(validationData);
        MulticlassClassificationMetrics metrics = _mlContext.MulticlassClassification.Evaluate(
            data: predictions,
            labelColumnName: LabelColumnName,
            predictedLabelColumnName: PredictedLabelColumnName);

        return new TrainingMetrics(
            microAccuracy: metrics.MicroAccuracy,
            macroAccuracy: metrics.MacroAccuracy,
            confusionMatrix: metrics.ConfusionMatrix.GetFormattedConfusionTable());
    }
}
