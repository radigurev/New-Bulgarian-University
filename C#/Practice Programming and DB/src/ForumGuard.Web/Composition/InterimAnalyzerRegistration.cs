using ForumGuard.Application.Moderation;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Analysis;
using Microsoft.Extensions.DependencyInjection;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Registers an interim <see cref="ICommentAnalyzer"/> Strategy under
/// <see cref="AnalyzerKeys.NasBert"/> so the moderation pipeline resolves and runs end-to-end before
/// the real NAS-BERT classifier ships (CLAUDE.md §6, SDD-FORUM-020).
/// <para>This is an explicit, temporary stand-in: the NAS-BERT key currently reuses the keyword
/// analyzer so both pipeline links perform keyword matching. Replacing it with the real
/// <c>NasBertCommentAnalyzer</c> is a one-line swap in <see cref="AddInterimNasBertAnalyzer"/>.</para>
/// </summary>
public static class InterimAnalyzerRegistration
{
    /// <summary>
    /// Registers the interim NAS-BERT analyzer keyed under <see cref="AnalyzerKeys.NasBert"/>.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// Temporary: the NAS-BERT key resolves to <see cref="KeywordCommentAnalyzer"/> until the ML
    /// increment introduces the trained classifier. Swap the implementation type below to migrate.
    /// </remarks>
    public static IServiceCollection AddInterimNasBertAnalyzer(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddKeyedSingleton<ICommentAnalyzer, KeywordCommentAnalyzer>(AnalyzerKeys.NasBert);

        return services;
    }
}
