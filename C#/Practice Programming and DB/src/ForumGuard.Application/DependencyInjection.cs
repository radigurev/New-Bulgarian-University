using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Comments;
using ForumGuard.Application.Comments.Interfaces;
using ForumGuard.Application.Moderation;
using ForumGuard.Application.Moderation.Handlers;
using ForumGuard.Application.Moderation.Interfaces;
using ForumGuard.Application.Options;
using ForumGuard.Application.Workspaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ForumGuard.Application;

/// <summary>
/// Registers the ForumGuard Application-layer services with the dependency-injection container.
/// <para>Registers use-case services, the moderation pipeline and handlers, the options validator,
/// the role workspaces and resolver, and the clock. Keyed <c>ICommentAnalyzer</c> implementations,
/// options binding, and <c>ValidateOnStart</c> are wired by the composition root (Infrastructure / Web).</para>
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Application-layer services to the supplied service collection.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        AddInfrastructureAbstractions(services);
        AddModerationPipeline(services);
        AddUseCaseServices(services);
        AddRoleWorkspaces(services);

        return services;
    }

    private static void AddInfrastructureAbstractions(IServiceCollection services)
    {
        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ModerationOptions>, ModerationOptionsValidator>());
    }

    private static void AddModerationPipeline(IServiceCollection services)
    {
        services.AddScoped<ICommentModerationHandler, ProfanityPreFilterHandler>();
        services.AddScoped<ICommentModerationHandler, MlToxicityHandler>();
        services.AddScoped<ICommentModerationPipeline, CommentModerationPipeline>();
    }

    private static void AddUseCaseServices(IServiceCollection services)
    {
        services.AddScoped<ICommentSubmissionService, CommentSubmissionService>();
        services.AddScoped<IModerationService, ModerationService>();
    }

    private static void AddRoleWorkspaces(IServiceCollection services)
    {
        services.AddSingleton<IRoleWorkspace, UserWorkspace>();
        services.AddSingleton<IRoleWorkspace, ModeratorWorkspace>();
        services.AddSingleton<IRoleWorkspace, AdministratorWorkspace>();
        services.AddSingleton<IRoleWorkspaceResolver, RoleWorkspaceResolver>();
    }
}
