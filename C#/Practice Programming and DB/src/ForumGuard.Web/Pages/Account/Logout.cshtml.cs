using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Account;

/// <summary>
/// Signs the current user out via an antiforgery-protected POST (SDD-FORUM-003).
/// <para>Delegates sign-out to <see cref="IAccountService"/>; no GET handler exists so sign-out is
/// never triggered by navigation.</para>
/// </summary>
[AllowAnonymous]
public sealed class LogoutModel : PageModel
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Initializes the page with the account service.
    /// </summary>
    /// <param name="accountService">The service performing sign-out.</param>
    public LogoutModel(IAccountService accountService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        _accountService = accountService;
    }

    /// <summary>
    /// Signs the user out and redirects to the home page.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect to the home page.</returns>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await _accountService.SignOutAsync(cancellationToken);
        return RedirectToPage("/Index");
    }
}
