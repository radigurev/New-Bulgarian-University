using System.ComponentModel.DataAnnotations;
using ForumGuard.Application.Common;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Account;

/// <summary>
/// Handles sign-in, enforcing the IsActive gate (SDD-FORUM-003 B6, E3, E4).
/// <para>Delegates credential and active-state checking to <see cref="IAccountService"/>; bad
/// credentials and a deactivated account surface as distinct, non-technical messages.</para>
/// </summary>
[AllowAnonymous]
public sealed class LoginModel : PageModel
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Initializes the page with the account service.
    /// </summary>
    /// <param name="accountService">The service performing sign-in.</param>
    public LoginModel(IAccountService accountService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        _accountService = accountService;
    }

    /// <summary>
    /// Gets or sets the bound sign-in form input.
    /// </summary>
    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    /// <summary>
    /// Gets or sets the local URL to return to after a successful sign-in.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Gets or sets the status message carried from a prior page (for example, after registering).
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Renders the sign-in form.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Processes the sign-in submission.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect on success, or the redisplayed form on failure.</returns>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        SignInRequest request = new(Input.Email, Input.Password, Input.RememberMe);
        Result<Guid> result = await _accountService.SignInAsync(request, cancellationToken);
        if (result.IsSuccess)
        {
            return RedirectToLocal();
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Invalid login attempt.");
        return Page();
    }

    private IActionResult RedirectToLocal()
    {
        if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl);
        }

        return RedirectToPage("/Dashboard");
    }

    /// <summary>
    /// Carries the sign-in form fields with client-side validation attributes.
    /// </summary>
    public sealed class LoginInputModel
    {
        /// <summary>
        /// Gets or sets the account email used as the username.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password to verify.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to issue a persistent authentication cookie.
        /// </summary>
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
