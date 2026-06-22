using ForumGuard.Infrastructure.Analysis;
using ForumGuard.ModelTrainer.Dataset;
using ForumGuard.ModelTrainer.Training;
using ForumGuard.Tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.ML;

namespace ForumGuard.Tests.ModelTrainer;

/// <summary>
/// Tests for <see cref="TrainingPipeline"/> covering SDD-FORUM-024 §2.1 / §2.3: the estimator-chain
/// construction is verified as a pure unit (no <c>Fit</c>, no native backend), while the
/// train-then-load-and-score round-trip is authored but marked <c>[Explicit]</c> +
/// <c>[Category("RequiresTraining")]</c> + <c>[Ignore]</c> so the default suite never downloads
/// <c>libtorch-cpu</c> or runs slow CPU fine-tuning.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-024")]
public sealed class TrainingPipelineTests
{
    private MLContext _mlContext = null!;

    [SetUp]
    public void SetUp()
    {
        _mlContext = new MLContext(seed: 1);
    }

    [Test]
    public void BuildPipeline_WithoutTraining_ReturnsNonNullEstimator()
    {
        // Arrange
        TrainingPipeline sut = new(_mlContext);

        // Act
        IEstimator<ITransformer> pipeline = sut.BuildPipeline();

        // Assert
        Assert.That(pipeline, Is.Not.Null);
    }

    [Test]
    public void Constructor_NullMlContext_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.That(() => new TrainingPipeline(null!), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void ColumnNames_MatchSpecContract_LabelAndTextAndPredictedLabel()
    {
        // Arrange, Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(TrainingPipeline.LabelColumnName, Is.EqualTo("Label"));
            Assert.That(TrainingPipeline.TextColumnName, Is.EqualTo("Text"));
            Assert.That(TrainingPipeline.PredictedLabelColumnName, Is.EqualTo("PredictedLabel"));
        });
    }

    [Test]
    public void Train_NullTrainingData_ThrowsArgumentNullExceptionWithoutFitting()
    {
        // Arrange
        TrainingPipeline sut = new(_mlContext);
        IDataView validationData = _mlContext.Data.LoadFromEnumerable(new List<TrainingDataRow>());

        // Act & Assert
        Assert.That(() => sut.Train(null!, validationData), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void Evaluate_NullTrainedModel_ThrowsArgumentNullException()
    {
        // Arrange
        TrainingPipeline sut = new(_mlContext);
        IDataView validationData = _mlContext.Data.LoadFromEnumerable(new List<TrainingDataRow>());

        // Act & Assert
        Assert.That(() => sut.Evaluate(null!, validationData), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    [Explicit("Requires libtorch-cpu + slow CPU training; run manually")]
    [Category("RequiresTraining")]
    [Ignore("Requires libtorch-cpu + slow CPU training; run manually")]
    public void Train_TinyTwoClassFixture_ThenLoadViaPredictionEnginePool_ScoresWithoutSchemaMismatch()
    {
        // Arrange
        List<TrainingDataRow> corpus = BuildTinyTwoClassCorpus();
        IDataView data = _mlContext.Data.LoadFromEnumerable(corpus);
        DataOperationsCatalog.TrainTestData split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
        TrainingPipeline pipeline = new(_mlContext);
        ModelExporter exporter = new(_mlContext);
        string modelPath = Path.Combine(Path.GetTempPath(), $"forumguard-trained-{Guid.NewGuid():N}.zip");

        // Act
        ITransformer trainedModel = pipeline.Train(split.TrainSet, split.TestSet);
        TrainingMetrics metrics = pipeline.Evaluate(trainedModel, split.TestSet);
        exporter.Export(trainedModel, data.Schema, modelPath);

        ServiceCollection services = [];
        services
            .AddPredictionEnginePool<ModelInput, ModelOutput>()
            .FromFile(modelName: "trained", filePath: modelPath, watchForChanges: false);
        using ServiceProvider provider = services.BuildServiceProvider();
        PredictionEnginePool<ModelInput, ModelOutput> pool =
            provider.GetRequiredService<PredictionEnginePool<ModelInput, ModelOutput>>();
        ModelOutput output = pool.Predict("trained", new ModelInput { Text = "you are an idiot and a loser" });

        // Assert
        try
        {
            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(modelPath), Is.True);
                Assert.That(metrics.ConfusionMatrix, Is.Not.Empty);
                Assert.That(output.Score, Is.Not.Empty);
                Assert.That(output.PredictedLabel, Is.AnyOf("Clean", "Toxic"));
            });
        }
        finally
        {
            if (File.Exists(modelPath))
            {
                File.Delete(modelPath);
            }
        }
    }

    /// <summary>
    /// Builds a tiny balanced two-class corpus for the explicit training round-trip.
    /// </summary>
    /// <returns>The training rows.</returns>
    private static List<TrainingDataRow> BuildTinyTwoClassCorpus()
    {
        List<TrainingDataRow> rows = [];
        for (int i = 0; i < 8; i++)
        {
            rows.Add(new TrainingDataRow { Text = $"thank you for the helpful and kind reply {i}", Label = "Clean" });
            rows.Add(new TrainingDataRow { Text = $"you are an idiot and everyone hates you {i}", Label = "Toxic" });
        }

        return rows;
    }
}
