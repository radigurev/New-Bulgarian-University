using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Application.Moderation;

/// <summary>
/// Provides the keyed-DI service keys that distinguish the two interchangeable comment analyzers.
/// <para>Each handler resolves its <see cref="ICommentAnalyzer"/> Strategy by one of these keys; the
/// composition root (Infrastructure / Web) registers the concrete analyzers against these same keys.</para>
/// </summary>
public static class AnalyzerKeys
{
    /// <summary>
    /// The key for the fast, model-free keyword analyzer used by the profanity pre-filter handler.
    /// </summary>
    public const string Keyword = "Keyword";

    /// <summary>
    /// The key for the NAS-BERT machine-learning analyzer used by the ML toxicity handler.
    /// </summary>
    public const string NasBert = "NasBert";
}
