using Microsoft.Extensions.Logging;

namespace ForumGuard.Application.Moderation;

/// <summary>
/// Assembles the configured moderation handlers into a chain and traverses it for one comment (SDD-FORUM-021).
/// <para>Short-circuits on the first flag, fails safe to flagged on handler error, and never propagates handler exceptions.</para>
/// <para>See <see cref="ICommentModerationHandler"/> and <see cref="ModerationVerdict"/>.</para>
/// </summary>
public sealed class CommentModerationPipeline : ICommentModerationPipeline
{
    private const float FailSafeScore = 1.0f;

    private readonly IReadOnlyList<ICommentModerationHandler> _orderedHandlers;
    private readonly ILogger<CommentModerationPipeline> _logger;

    /// <summary>
    /// Initializes the pipeline with the registered handlers, ordered by their configured position.
    /// </summary>
    /// <param name="handlers">The handlers composing the chain; traversed in ascending <see cref="ICommentModerationHandler.Order"/>.</param>
    /// <param name="logger">The logger used to record fail-safe handler exceptions.</param>
    public CommentModerationPipeline(
        IEnumerable<ICommentModerationHandler> handlers,
        ILogger<CommentModerationPipeline> logger)
    {
        ArgumentNullException.ThrowIfNull(handlers);
        ArgumentNullException.ThrowIfNull(logger);
        _orderedHandlers = handlers.OrderBy(handler => handler.Order).ToArray();
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ModerationVerdict> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        float lastObservedScore = 0.0f;

        foreach (ICommentModerationHandler handler in _orderedHandlers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            HandlerEvaluation evaluation = await EvaluateHandlerAsync(handler, context, cancellationToken).ConfigureAwait(false);
            if (evaluation.Verdict is not null)
            {
                return evaluation.Verdict;
            }

            lastObservedScore = evaluation.ObservedScore;
        }

        return ModerationVerdict.Clean(lastObservedScore);
    }

    private async Task<HandlerEvaluation> EvaluateHandlerAsync(
        ICommentModerationHandler handler,
        CommentModerationContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            HandlerResult result = await handler.EvaluateAsync(context, cancellationToken).ConfigureAwait(false);
            ModerationVerdict? verdict = result.IsFlag ? ModerationVerdict.Flagged(handler.Name, result.Score) : null;
            return new HandlerEvaluation(verdict, result.Score);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Moderation handler {HandlerName} threw while evaluating comment {CommentId}; failing safe to flagged.",
                handler.Name,
                context.CommentId);

            return new HandlerEvaluation(ModerationVerdict.Flagged(handler.Name, FailSafeScore), FailSafeScore);
        }
    }
}
