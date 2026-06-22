using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Adapts the ML.NET <see cref="PredictionEnginePool{ModelInput, ModelOutput}"/> Object-Pool to the
/// <see cref="ICommentAnalyzer"/> Strategy, scoring comment text with the trained NAS-BERT model.
/// <para>See SDD-FORUM-020. The toxic-class probability is read from <see cref="ModelOutput.Score"/>
/// and compared against <see cref="ModerationOptions.ToxicityThreshold"/> using an inclusive
/// <c>&gt;=</c>: at or above the threshold yields <see cref="ToxicityLabel.Toxic"/>, otherwise
/// <see cref="ToxicityLabel.Clean"/>. Null/empty/whitespace text short-circuits to
/// <see cref="ToxicityLabel.Clean"/> with score <c>0.0</c> without invoking the model. A missing or
/// unloadable model, or any inference failure, raises <see cref="AnalyzerUnavailableException"/>;
/// the analyzer never silently returns <see cref="ToxicityLabel.Clean"/> on failure.</para>
/// </summary>
public sealed class NasBertCommentAnalyzer : ICommentAnalyzer
{
    private static readonly AnalysisResult CleanResult = new(ToxicityLabel.Clean, 0.0f);

    private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEnginePool;
    private readonly IOptionsMonitor<ModerationOptions> _options;
    private readonly string _modelName;

    /// <summary>
    /// Initializes the analyzer with the injected prediction-engine pool and the moderation options monitor.
    /// </summary>
    /// <param name="predictionEnginePool">The pool that rents thread-safe prediction engines for the model.</param>
    /// <param name="options">The monitor supplying the operator-tunable toxicity threshold.</param>
    /// <param name="modelName">The logical model name the pool was registered under.</param>
    public NasBertCommentAnalyzer(
        PredictionEnginePool<ModelInput, ModelOutput> predictionEnginePool,
        IOptionsMonitor<ModerationOptions> options,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(predictionEnginePool);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

        _predictionEnginePool = predictionEnginePool;
        _options = options;
        _modelName = modelName;
    }

    /// <inheritdoc />
    public Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(CleanResult);
        }

        ModelOutput output = Predict(text);
        cancellationToken.ThrowIfCancellationRequested();

        float toxicScore = ExtractToxicScore(output);
        return Task.FromResult(BuildResult(toxicScore));
    }

    /// <summary>
    /// Rents an engine from the pool and scores the supplied text, wrapping any failure as an
    /// <see cref="AnalyzerUnavailableException"/> so a missing/unloadable model is never treated as clean.
    /// </summary>
    /// <param name="text">The non-empty comment text to score.</param>
    /// <returns>The raw model output for the supplied text.</returns>
    /// <exception cref="AnalyzerUnavailableException">Thrown when the model cannot be loaded or scoring fails.</exception>
    private ModelOutput Predict(string text)
    {
        try
        {
            return _predictionEnginePool.Predict(_modelName, new ModelInput { Text = text });
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new AnalyzerUnavailableException(
                $"The NAS-BERT model '{_modelName}' could not score the comment; the analyzer is unavailable.",
                exception);
        }
    }

    /// <summary>
    /// Builds the <see cref="AnalysisResult"/> from the toxic-class probability using the inclusive
    /// threshold drawn from <see cref="ModerationOptions.ToxicityThreshold"/>.
    /// </summary>
    /// <param name="toxicScore">The toxic-class probability in the inclusive range <c>[0.0, 1.0]</c>.</param>
    /// <returns>The classification result.</returns>
    private AnalysisResult BuildResult(float toxicScore)
    {
        float threshold = _options.CurrentValue.ToxicityThreshold;
        ToxicityLabel label = toxicScore >= threshold ? ToxicityLabel.Toxic : ToxicityLabel.Clean;
        return new AnalysisResult(label, toxicScore);
    }

    /// <summary>
    /// Extracts the toxic-class probability from the model output's score vector and clamps it into
    /// the inclusive range <c>[0.0, 1.0]</c> expected by <see cref="AnalysisResult"/>.
    /// </summary>
    /// <param name="output">The raw model output.</param>
    /// <returns>The toxic-class probability.</returns>
    /// <exception cref="AnalyzerUnavailableException">Thrown when the score vector is missing or empty.</exception>
    private static float ExtractToxicScore(ModelOutput output)
    {
        if (output.Score is null || output.Score.Length == 0)
        {
            throw new AnalyzerUnavailableException(
                "The NAS-BERT model returned an empty score vector; the analyzer is unavailable.");
        }

        int toxicIndex = ResolveToxicIndex(output);
        float rawScore = output.Score[toxicIndex];
        return Clamp(rawScore);
    }

    /// <summary>
    /// Resolves the index within the score vector that corresponds to the <c>Toxic</c> class. The
    /// binary label space is <c>{ Clean, Toxic }</c>; when the predicted label is <c>Toxic</c> its
    /// own probability is the toxic score, otherwise the toxic class is the remaining slot.
    /// </summary>
    /// <param name="output">The raw model output.</param>
    /// <returns>The toxic-class index within <see cref="ModelOutput.Score"/>.</returns>
    private static int ResolveToxicIndex(ModelOutput output)
    {
        if (output.Score.Length == 1)
        {
            return 0;
        }

        bool predictedToxic = string.Equals(
            output.PredictedLabel, ToxicityLabel.Toxic.ToString(), StringComparison.OrdinalIgnoreCase);

        return predictedToxic
            ? ArgMax(output.Score)
            : ArgMin(output.Score);
    }

    private static int ArgMax(float[] scores)
    {
        int index = 0;
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] > scores[index])
            {
                index = i;
            }
        }

        return index;
    }

    private static int ArgMin(float[] scores)
    {
        int index = 0;
        for (int i = 1; i < scores.Length; i++)
        {
            if (scores[i] < scores[index])
            {
                index = i;
            }
        }

        return index;
    }

    private static float Clamp(float value)
    {
        if (float.IsNaN(value))
        {
            return 0.0f;
        }

        return Math.Clamp(value, 0.0f, 1.0f);
    }
}
