using ForumGuard.Web.Composition.Interfaces;
using ForumGuard.Web.Services;
using ForumGuard.Web.Services.Interfaces;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Registers the Web-layer service classes that keep Razor Page handlers thin (SDD-FORUM-001..004).
/// <para>These services encapsulate Identity-backed account and role management, thread creation, and
/// read projections so page handlers delegate rather than embed logic.</para>
/// </summary>
public static class WebServicesRegistration
{
    /// <summary>
    /// Adds the Web-layer services and the startup data seeder to the service collection.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IModeratorRoleService, ModeratorRoleService>();
        services.AddScoped<IForumReadService, ForumReadService>();
        services.AddScoped<IThreadCreationService, ThreadCreationService>();
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
