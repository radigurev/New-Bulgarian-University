namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Thrown when the NAS-BERT comment analyzer cannot produce a classification because the underlying
/// model is missing, unloadable, or the ML.NET prediction call fails.
/// <para>See SDD-FORUM-020 (E1, E2, E4). This exception enforces the fail-safe principle: an analyzer
/// failure MUST NEVER be silently converted into a <c>Clean</c> result. The submission flow
/// (SDD-FORUM-001) and the moderation pipeline (SDD-FORUM-021) treat a thrown
/// <see cref="AnalyzerUnavailableException"/> as "not analyzable" and withhold the comment rather than
/// auto-publishing it.</para>
/// </summary>
public sealed class AnalyzerUnavailableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzerUnavailableException"/> class.
    /// </summary>
    /// <param name="message">The message describing why the analyzer is unavailable.</param>
    public AnalyzerUnavailableException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzerUnavailableException"/> class wrapping an inner cause.
    /// </summary>
    /// <param name="message">The message describing why the analyzer is unavailable.</param>
    /// <param name="innerException">The underlying failure (IO, model-load, or ML.NET inference exception).</param>
    public AnalyzerUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
