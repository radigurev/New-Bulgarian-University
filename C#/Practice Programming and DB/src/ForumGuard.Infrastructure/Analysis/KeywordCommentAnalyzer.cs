using ForumGuard.Application.Options;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// A fast, model-free <see cref="ICommentAnalyzer"/> Strategy that flags comments matching any
/// keyword in the configured profanity list, case-insensitively.
/// <para>See SDD-FORUM-020. A match yields <see cref="ToxicityLabel.Toxic"/> with score <c>1.0</c>;
/// otherwise <see cref="ToxicityLabel.Clean"/> with score <c>0.0</c>. Null/empty/whitespace text
/// short-circuits to <see cref="ToxicityLabel.Clean"/> without matching. A missing or unreadable
/// profanity list raises <see cref="AnalyzerConfigurationException"/> at construction.</para>
/// </summary>
public sealed class KeywordCommentAnalyzer : ICommentAnalyzer
{
    private static readonly AnalysisResult CleanResult = new(ToxicityLabel.Clean, 0.0f);
    private static readonly AnalysisResult ToxicResult = new(ToxicityLabel.Toxic, 1.0f);

    private readonly IReadOnlyList<string> _keywords;

    /// <summary>
    /// Initializes the analyzer by loading the profanity keyword list from
    /// <see cref="ModerationOptions.ProfanityListPath"/>.
    /// </summary>
    /// <param name="options">The moderation options supplying the profanity list path.</param>
    /// <exception cref="AnalyzerConfigurationException">Thrown when the path is unset or the file cannot be read.</exception>
    public KeywordCommentAnalyzer(IOptions<ModerationOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _keywords = LoadKeywords(options.Value.ProfanityListPath);
    }

    /// <inheritdoc />
    public Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(text))
        {
            return Task.FromResult(CleanResult);
        }

        bool isToxic = ContainsKeyword(text);
        return Task.FromResult(isToxic ? ToxicResult : CleanResult);
    }

    private bool ContainsKeyword(string text)
    {
        foreach (string keyword in _keywords)
        {
            if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<string> LoadKeywords(string profanityListPath)
    {
        if (string.IsNullOrWhiteSpace(profanityListPath))
        {
            throw new AnalyzerConfigurationException(
                "Moderation:ProfanityListPath is not configured; the keyword analyzer cannot initialize.");
        }

        string resolvedPath = ResolveProfanityListPath(profanityListPath);

        if (!File.Exists(resolvedPath))
        {
            throw new AnalyzerConfigurationException(
                $"The profanity list file was not found at '{resolvedPath}'.");
        }

        return ReadKeywordsFromFile(resolvedPath);
    }

    /// <summary>
    /// Resolves a configured profanity-list path. Absolute paths are used as-is; a relative path is
    /// anchored to <see cref="AppContext.BaseDirectory"/> so the deployed asset is found regardless of
    /// the process working directory.
    /// </summary>
    /// <param name="profanityListPath">The configured profanity-list path.</param>
    /// <returns>An absolute path to the profanity list file.</returns>
    private static string ResolveProfanityListPath(string profanityListPath)
    {
        return Path.IsPathRooted(profanityListPath)
            ? profanityListPath
            : Path.Combine(AppContext.BaseDirectory, profanityListPath);
    }

    private static IReadOnlyList<string> ReadKeywordsFromFile(string profanityListPath)
    {
        try
        {
            string[] lines = File.ReadAllLines(profanityListPath);
            return lines
                .Select(line => line.Trim())
                .Where(line => line.Length > 0 && !line.StartsWith('#'))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch (IOException exception)
        {
            throw new AnalyzerConfigurationException(
                $"The profanity list file at '{profanityListPath}' could not be read.", exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new AnalyzerConfigurationException(
                $"The profanity list file at '{profanityListPath}' could not be read.", exception);
        }
    }
}
