using System.Globalization;
using ForumGuard.Infrastructure.Analysis;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace ForumGuard.Tests.Fixtures;

/// <summary>
/// Builds tiny, purely managed ML.NET <c>model.zip</c> artifacts that round-trip through
/// <c>PredictionEnginePool&lt;ModelInput, ModelOutput&gt;.FromFile</c> without the native
/// <c>libtorch-cpu</c> backend and without any NAS-BERT training.
/// <para>The saved models are built from a deterministic <see cref="CustomMapping{TSrc, TDst}"/>
/// transform (managed code only). They let the SDD-FORUM-020 <see cref="NasBertCommentAnalyzer"/>
/// tests exercise the genuine pool-backed prediction path with controllable, repeatable outputs:
/// a <c>score:</c>-encoded model that emits an exact toxic probability, a model that throws inside
/// the mapping (simulating an inference failure), and a model that returns an empty score vector.</para>
/// </summary>
public static class ManagedToxicityModelFixture
{
    /// <summary>
    /// The logical model name the prediction-engine pool is registered under in the tests.
    /// </summary>
    public const string ModelName = "TestToxicityModel";

    private const string ScorePrefix = "score:";
    private const string ThrowSentinel = "throw-now";
    private const string EmptyScoreSentinel = "empty-score";

    /// <summary>
    /// Saves a deterministic toxicity model to a unique temporary <c>model.zip</c> and returns its path.
    /// </summary>
    /// <returns>The path to the saved managed model artifact.</returns>
    public static string CreateModelFile()
    {
        string modelPath = Path.Combine(
            Path.GetTempPath(), $"forumguard-test-model-{Guid.NewGuid():N}.zip");

        MLContext mlContext = new(seed: 1);
        mlContext.ComponentCatalog.RegisterAssembly(typeof(TestToxicityMapping).Assembly);

        IDataView emptyData = mlContext.Data.LoadFromEnumerable(new List<ModelInput>());
        CustomMappingEstimator<ModelInput, ModelOutput> estimator =
            mlContext.Transforms.CustomMapping(new TestToxicityMapping().GetMapping(), TestToxicityMapping.ContractName);
        ITransformer model = estimator.Fit(emptyData);

        mlContext.Model.Save(model, emptyData.Schema, modelPath);
        return modelPath;
    }

    /// <summary>
    /// Builds the model input text that drives the deterministic model to emit the supplied toxic score.
    /// </summary>
    /// <param name="toxicScore">The toxic-class probability the model should report.</param>
    /// <returns>The encoded text to pass to <c>AnalyzeAsync</c>.</returns>
    public static string EncodeToxicScore(float toxicScore)
    {
        return ScorePrefix + toxicScore.ToString("R", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Gets the sentinel text that makes the model's mapping throw, simulating an inference failure.
    /// </summary>
    public static string ThrowingText => ThrowSentinel;

    /// <summary>
    /// Gets the sentinel text that makes the model return an empty score vector.
    /// </summary>
    public static string EmptyScoreText => EmptyScoreSentinel;

    /// <summary>
    /// Deterministic managed mapping from <see cref="ModelInput"/> to <see cref="ModelOutput"/> used to
    /// stand in for the trained NAS-BERT transformer in unit tests.
    /// </summary>
    [CustomMappingFactoryAttribute(ContractName)]
    public sealed class TestToxicityMapping : CustomMappingFactory<ModelInput, ModelOutput>
    {
        /// <summary>
        /// The contract name under which this mapping is registered and persisted in the model.
        /// </summary>
        public const string ContractName = "ForumGuardTestToxicityMapping";

        /// <summary>
        /// Maps an input to a deterministic output: a <c>score:</c>-encoded toxic probability, a thrown
        /// failure, or an empty score vector depending on the input sentinel.
        /// </summary>
        /// <param name="input">The model input.</param>
        /// <param name="output">The model output to populate.</param>
        public static void Map(ModelInput input, ModelOutput output)
        {
            string text = input.Text ?? string.Empty;

            if (text.Contains(ThrowSentinel, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Simulated NAS-BERT inference failure inside the mapping.");
            }

            if (text.Contains(EmptyScoreSentinel, StringComparison.Ordinal))
            {
                output.PredictedLabel = "Clean";
                output.Score = [];
                return;
            }

            float toxicScore = ExtractEncodedScore(text);
            output.PredictedLabel = toxicScore >= 0.5f ? "Toxic" : "Clean";
            output.Score = [1.0f - toxicScore, toxicScore];
        }

        /// <inheritdoc />
        public override Action<ModelInput, ModelOutput> GetMapping() => Map;

        private static float ExtractEncodedScore(string text)
        {
            int prefixIndex = text.IndexOf(ScorePrefix, StringComparison.Ordinal);
            if (prefixIndex < 0)
            {
                return 0.0f;
            }

            string scoreText = text[(prefixIndex + ScorePrefix.Length)..];
            return float.TryParse(scoreText, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed)
                ? parsed
                : 0.0f;
        }
    }
}
