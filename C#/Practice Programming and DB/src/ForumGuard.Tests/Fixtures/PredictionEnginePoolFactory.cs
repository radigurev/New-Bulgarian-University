using ForumGuard.Infrastructure.Analysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;

namespace ForumGuard.Tests.Fixtures;

/// <summary>
/// Resolves a real <see cref="PredictionEnginePool{ModelInput, ModelOutput}"/> over a managed model
/// file so the SDD-FORUM-020 analyzer tests drive the genuine pool-backed prediction path.
/// <para>The pool is registered via <c>AddPredictionEnginePool().FromFile</c> exactly as production
/// does, but against a tiny purely managed artifact built by <see cref="ManagedToxicityModelFixture"/>
/// — no native <c>libtorch-cpu</c> backend and no training are involved.</para>
/// </summary>
public static class PredictionEnginePoolFactory
{
    /// <summary>
    /// Builds a service provider that exposes a prediction-engine pool loaded from the supplied model.
    /// </summary>
    /// <param name="modelPath">The managed <c>model.zip</c> path to load.</param>
    /// <param name="modelName">The logical model name to register the pool under.</param>
    /// <returns>The configured service provider; dispose it to release the pool.</returns>
    public static ServiceProvider BuildProvider(string modelPath, string modelName)
    {
        ServiceCollection services = [];
        services
            .AddPredictionEnginePool<ModelInput, ModelOutput>()
            .FromFile(modelName: modelName, filePath: modelPath, watchForChanges: false);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolves the prediction-engine pool from a provider built over the supplied model.
    /// </summary>
    /// <param name="provider">The provider built by <see cref="BuildProvider"/>.</param>
    /// <returns>The resolved pool.</returns>
    public static PredictionEnginePool<ModelInput, ModelOutput> ResolvePool(ServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        return provider.GetRequiredService<PredictionEnginePool<ModelInput, ModelOutput>>();
    }
}
