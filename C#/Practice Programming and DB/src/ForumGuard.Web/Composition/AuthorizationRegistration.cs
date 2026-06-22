using ForumGuard.Web.Authorization;
using ForumGuard.Web.Authorization.Handlers;
using ForumGuard.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Registers the three custom authorization policies and their handlers (SDD-FORUM-011 B-13, B-14).
/// <para>Binds <see cref="ForumPolicies"/> names to their requirements and registers each
/// <see cref="IAuthorizationHandler"/> as a singleton.</para>
/// </summary>
public static class AuthorizationRegistration
{
    /// <summary>
    /// Adds the ForumGuard authorization policies and handlers to the service collection.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddForumAuthorization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IAuthorizationHandler, CanModerateCommentsHandler>();
        services.AddSingleton<IAuthorizationHandler, CanManageModeratorsHandler>();
        services.AddSingleton<IAuthorizationHandler, CanManageAccountsHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy(ForumPolicies.CanModerateComments, policy =>
                policy.Requirements.Add(new CanModerateCommentsRequirement()))
            .AddPolicy(ForumPolicies.CanManageModerators, policy =>
                policy.Requirements.Add(new CanManageModeratorsRequirement()))
            .AddPolicy(ForumPolicies.CanManageAccounts, policy =>
                policy.Requirements.Add(new CanManageAccountsRequirement()));

        return services;
    }
}
