using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Registers ASP.NET Core Identity exactly once for cookie sign-in usable by Razor Pages
/// (SDD-FORUM-003). The Infrastructure layer registers the DbContext and stores only; the full
/// Identity stack (with <see cref="SignInManager{TUser}"/>) and the application cookie live here.
/// <para>Also wires the per-request IsActive re-check via
/// <see cref="ActiveUserCookieValidator"/> for mid-session deactivation (B14, B15).</para>
/// </summary>
public static class IdentityRegistration
{
    /// <summary>
    /// Adds the full Identity stack, the application cookie, and the IsActive validator.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddForumIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<ForumGuardDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<ActiveUserCookieValidator>();
        services.ConfigureApplicationCookie(ConfigureCookie);

        return services;
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    }

    private static void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events.OnValidatePrincipal = context =>
            context.HttpContext.RequestServices
                .GetRequiredService<ActiveUserCookieValidator>()
                .ValidateAsync(context);
    }
}
