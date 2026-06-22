using ForumGuard.Application.Moderation;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// Configurable <see cref="ICommentModerationHandler"/> test double that records traversal and can flag, pass, or throw.
/// <para>Records the relative invocation sequence to verify ordering and short-circuit semantics (SDD-FORUM-021).</para>
/// </summary>
public sealed class RecordingModerationHandler : ICommentModerationHandler
{
    private static int _globalSequence;

    private readonly HandlerResult? _result;
    private readonly bool _throws;

    private RecordingModerationHandler(string name, int order, HandlerResult? result, bool throws)
    {
        Name = name;
        Order = order;
        _result = result;
        _throws = throws;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public int Order { get; }

    /// <summary>
    /// Gets the number of times this handler was evaluated.
    /// </summary>
    public int InvocationCount { get; private set; }

    /// <summary>
    /// Gets the global sequence number recorded when this handler last ran, or <c>-1</c> if it never ran.
    /// </summary>
    public int ObservedSequence { get; private set; } = -1;

    /// <summary>
    /// Creates a handler that passes control to the next link, reporting the supplied score.
    /// </summary>
    /// <param name="name">The handler name.</param>
    /// <param name="order">The chain position.</param>
    /// <param name="score">The toxic-score the handler reports.</param>
    /// <returns>A passing handler.</returns>
    public static RecordingModerationHandler Passing(string name, int order, float score = 0.1f) =>
        new(name, order, HandlerResult.Pass(score), throws: false);

    /// <summary>
    /// Creates a handler that flags and short-circuits the chain, reporting the supplied score.
    /// </summary>
    /// <param name="name">The handler name.</param>
    /// <param name="order">The chain position.</param>
    /// <param name="score">The toxic-score that triggers the flag.</param>
    /// <returns>A flagging handler.</returns>
    public static RecordingModerationHandler Flagging(string name, int order, float score = 0.95f) =>
        new(name, order, HandlerResult.Flag(score), throws: false);

    /// <summary>
    /// Creates a handler that throws when evaluated, to exercise the pipeline's fail-safe path.
    /// </summary>
    /// <param name="name">The handler name.</param>
    /// <param name="order">The chain position.</param>
    /// <returns>A throwing handler.</returns>
    public static RecordingModerationHandler Throwing(string name, int order) =>
        new(name, order, result: null, throws: true);

    /// <summary>
    /// Resets the shared traversal sequence counter; call at the start of each ordering test.
    /// </summary>
    public static void ResetSequence() => _globalSequence = 0;

    /// <inheritdoc />
    public Task<HandlerResult> EvaluateAsync(CommentModerationContext context, CancellationToken cancellationToken = default)
    {
        InvocationCount++;
        ObservedSequence = _globalSequence++;

        if (_throws)
        {
            throw new InvalidOperationException($"Handler {Name} failure for fail-safe test.");
        }

        return Task.FromResult(_result!);
    }
}
