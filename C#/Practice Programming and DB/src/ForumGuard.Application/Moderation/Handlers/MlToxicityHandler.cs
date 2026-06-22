using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ForumGuard.Application.Moderation.Handlers;

/// <summary>
/// Second chain link (SDD-FORUM-021): flags a comment when the NAS-BERT toxic-score meets the configured threshold.
/// <para>Consumes the NAS-BERT <see cref="ICommentAnalyzer"/> Strategy resolved by <see cref="AnalyzerKeys.NasBert"/>
/// and reads the inclusive cutoff from <see cref="ModerationOptions.ToxicityThreshold"/>.</para>
/// </summary>
public sealed class MlToxicityHandler : ICommentModerationHandler
{
    private readonly ICommentAnalyzer _nasBertAnalyzer;
    private readonly IOptionsMonitor<ModerationOptions> _options;

    /// <summary>
    /// Initializes the handler with the keyed NAS-BERT analyzer and the moderation options monitor.
    /// </summary>
    /// <param name="nasBertAnalyzer">The NAS-BERT analyzer Strategy resolved by <see cref="AnalyzerKeys.NasBert"/>.</param>
    /// <param name="options">The monitor supplying the operator-tunable toxicity threshold.</param>
    public MlToxicityHandler(
        [FromKeyedServices(AnalyzerKeys.NasBert)] ICommentAnalyzer nasBertAnalyzer,
        IOptionsMonitor<ModerationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(nasBertAnalyzer);
        ArgumentNullException.ThrowIfNull(options);
        _nasBertAnalyzer = nasBertAnalyzer;
        _options = options;
    }

    /// <inheritdoc />
    public string Name => nameof(MlToxicityHandler);

    /// <inheritdoc />
    public int Order => 200;

    /// <inheritdoc />
    public async Task<HandlerResult> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        AnalysisResult result = await _nasBertAnalyzer
            .AnalyzeAsync(context.SafeBody, cancellationToken)
            .ConfigureAwait(false);

        float threshold = _options.CurrentValue.ToxicityThreshold;

        return result.Score >= threshold
            ? HandlerResult.Flag(result.Score)
            : HandlerResult.Pass(result.Score);
    }
}
