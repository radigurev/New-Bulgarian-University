using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Interfaces;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// <see cref="ICommentAnalyzer"/> test double that throws on analysis to exercise fail-safe behavior.
/// <para>Used to verify that an analyzer/handler exception is converted to a flagged verdict, never clean.</para>
/// </summary>
public sealed class ThrowingCommentAnalyzer : ICommentAnalyzer
{
    /// <inheritdoc />
    public Task<AnalysisResult> AnalyzeAsync(string text, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("Analyzer failure for fail-safe test.");
}
