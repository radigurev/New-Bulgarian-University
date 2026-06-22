using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Account;

/// <summary>
/// Renders the access-denied page shown when an authenticated user fails a policy (SDD-FORUM-011 E-1).
/// </summary>
[AllowAnonymous]
public sealed class AccessDeniedModel : PageModel
{
    /// <summary>
    /// Renders the access-denied message.
    /// </summary>
    public void OnGet()
    {
    }
}
