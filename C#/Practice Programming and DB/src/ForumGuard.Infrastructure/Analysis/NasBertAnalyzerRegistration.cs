using ForumGuard.Application.Moderation;
using ForumGuard.Application.Options;
using ForumGuard.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

namespace ForumGuard.Infrastructure.Analysis;

/// <summary>
/// Opt-in registration for the production NAS-BERT comment analyzer (SDD-FORUM-020).
/// <para>This registration is <b>not</b> wired by default. The Web composition root keeps the interim
/// keyword analyzer active (<c>AddInterimNasBertAnalyzer</c>) so the application runs end-to-end
/// without a trained <c>model.zip</c> or the native <c>libtorch-cpu</c> backend. Once a model has been
/// trained and <c>Moderation:ModelPath</c> points at it, the documented one-line swap in Web is to
/// replace <c>AddInterimNasBertAnalyzer()</c> with <c>AddNasBertCommentAnalyzer(Configuration)</c>.
/// See <c>src/ForumGuard.ModelTrainer/README.md</c> for the full enable checklist.</para>
/// </summary>
public static class NasBertAnalyzerRegistration
{
    /// <summary>
    /// The logical model name the prediction-engine pool is registered under.
    /// </summary>
    public const string ModelName = "NasBertToxicity";

    /// <summary>
    /// Registers the ML.NET <see cref="PredictionEnginePool{ModelInput, ModelOutput}"/> from the model
    /// file at <see cref="ModerationOptions.ModelPath"/> (with <c>watchForChanges: true</c> for
    /// hot-reload) and the <see cref="NasBertCommentAnalyzer"/> as the keyed
    /// <see cref="ICommentAnalyzer"/> under <see cref="AnalyzerKeys.NasBert"/>.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration supplying <c>Moderation:ModelPath</c>.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// Enabling this replaces the interim keyword stand-in for the <see cref="AnalyzerKeys.NasBert"/>
    /// key. Startup remains successful even when the model file is absent; with
    /// <c>watchForChanges: true</c> the pool picks the model up once it appears, and any analysis
    /// requested before the model is loadable surfaces as <see cref="AnalyzerUnavailableException"/>
    /// rather than a silent clean result (SDD-FORUM-020 §2.5).
    /// </remarks>
    public static IServiceCollection AddNasBertCommentAnalyzer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string modelPath = ResolveModelPath(configuration);

        services
            .AddPredictionEnginePool<ModelInput, ModelOutput>()
            .FromFile(modelName: ModelName, filePath: modelPath, watchForChanges: true);

        services.AddKeyedSingleton<ICommentAnalyzer>(AnalyzerKeys.NasBert, CreateAnalyzer);

        return services;
    }

    /// <summary>
    /// Resolves the configured <c>Moderation:ModelPath</c>, anchoring a relative path to the
    /// application base directory so the model is found regardless of the process working directory.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>An absolute path to the model file.</returns>
    private static string ResolveModelPath(IConfiguration configuration)
    {
        string? modelPath = configuration[ModerationConfigKeys.ModelPath];
        if (string.IsNullOrWhiteSpace(modelPath))
        {
            throw new AnalyzerConfigurationException(
                $"'{ModerationConfigKeys.ModelPath}' is not configured; the NAS-BERT analyzer cannot be registered.");
        }

        return Path.IsPathRooted(modelPath)
            ? modelPath
            : Path.Combine(AppContext.BaseDirectory, modelPath);
    }

    /// <summary>
    /// Factory that builds the keyed <see cref="NasBertCommentAnalyzer"/> from container services.
    /// </summary>
    /// <param name="serviceProvider">The container resolving the pool and options monitor.</param>
    /// <param name="serviceKey">The DI service key (the NAS-BERT analyzer key).</param>
    /// <returns>The constructed analyzer.</returns>
    private static NasBertCommentAnalyzer CreateAnalyzer(IServiceProvider serviceProvider, object? serviceKey)
    {
        PredictionEnginePool<ModelInput, ModelOutput> pool =
            serviceProvider.GetRequiredService<PredictionEnginePool<ModelInput, ModelOutput>>();
        IOptionsMonitor<ModerationOptions> options =
            serviceProvider.GetRequiredService<IOptionsMonitor<ModerationOptions>>();

        return new NasBertCommentAnalyzer(pool, options, ModelName);
    }
}
