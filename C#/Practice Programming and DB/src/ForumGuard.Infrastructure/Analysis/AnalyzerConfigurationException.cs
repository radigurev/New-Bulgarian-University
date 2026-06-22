namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Thrown when a comment analyzer is misconfigured, for example when the profanity keyword list
/// cannot be located or read.
/// <para>See SDD-FORUM-020 (E3). The moderation pipeline (SDD-FORUM-021) must fail closed rather
/// than treat a misconfigured analyzer as producing a clean result.</para>
/// </summary>
public sealed class AnalyzerConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzerConfigurationException"/> class.
    /// </summary>
    /// <param name="message">The message describing the misconfiguration.</param>
    public AnalyzerConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzerConfigurationException"/> class wrapping an inner cause.
    /// </summary>
    /// <param name="message">The message describing the misconfiguration.</param>
    /// <param name="innerException">The underlying cause of the misconfiguration.</param>
    public AnalyzerConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
