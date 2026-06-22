using ForumGuard.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ForumGuard.Web.Security;

/// <summary>
/// Re-evaluates <c>ApplicationUser.IsActive</c> from the persisted store on each authenticated
/// request and signs out any account deactivated mid-session (SDD-FORUM-003 B14, B15, E9).
/// <para>Wired to <see cref="CookieAuthenticationEvents.OnValidatePrincipal"/>; a stale cookie for a
/// now-inactive user no longer grants access.</para>
/// </summary>
public sealed class ActiveUserCookieValidator
{
    /// <summary>
    /// Validates the principal of an incoming request, rejecting deactivated accounts.
    /// </summary>
    /// <param name="context">The cookie validation context for the current request.</param>
    /// <returns>A task representing the asynchronous validation.</returns>
    public async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        UserManager<ApplicationUser> userManager = context.HttpContext.RequestServices
            .GetRequiredService<UserManager<ApplicationUser>>();

        ApplicationUser? user = await userManager.GetUserAsync(context.Principal!).ConfigureAwait(false);
        if (user is not null && user.IsActive)
        {
            return;
        }

        SignInManager<ApplicationUser> signInManager = context.HttpContext.RequestServices
            .GetRequiredService<SignInManager<ApplicationUser>>();

        context.RejectPrincipal();
        await signInManager.SignOutAsync().ConfigureAwait(false);
    }
}
