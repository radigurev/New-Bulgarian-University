using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ForumGuard.Application.Moderation.Handlers;

/// <summary>
/// First chain link (SDD-FORUM-021): flags obvious profanity via the keyword analyzer without invoking the ML model.
/// <para>Consumes the keyword <see cref="ICommentAnalyzer"/> Strategy resolved by <see cref="AnalyzerKeys.Keyword"/>.</para>
/// </summary>
public sealed class ProfanityPreFilterHandler : ICommentModerationHandler
{
    private readonly ICommentAnalyzer _keywordAnalyzer;

    /// <summary>
    /// Initializes the handler with the keyed keyword analyzer.
    /// </summary>
    /// <param name="keywordAnalyzer">The keyword analyzer Strategy resolved by <see cref="AnalyzerKeys.Keyword"/>.</param>
    public ProfanityPreFilterHandler(
        [FromKeyedServices(AnalyzerKeys.Keyword)] ICommentAnalyzer keywordAnalyzer)
    {
        ArgumentNullException.ThrowIfNull(keywordAnalyzer);
        _keywordAnalyzer = keywordAnalyzer;
    }

    /// <inheritdoc />
    public string Name => nameof(ProfanityPreFilterHandler);

    /// <inheritdoc />
    public int Order => 100;

    /// <inheritdoc />
    public async Task<HandlerResult> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        AnalysisResult result = await _keywordAnalyzer
            .AnalyzeAsync(context.SafeBody, cancellationToken)
            .ConfigureAwait(false);

        return result.Label == ToxicityLabel.Toxic
            ? HandlerResult.Flag(result.Score)
            : HandlerResult.Pass(result.Score);
    }
}
