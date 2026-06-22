using System.ComponentModel.DataAnnotations;
using ForumGuard.Application.Common;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Account;

/// <summary>
/// Handles self-registration, creating an active account in the User role (SDD-FORUM-003 B1..B5).
/// <para>Delegates all account logic to <see cref="IAccountService"/>; the handler only binds input
/// and translates the <see cref="Result{T}"/> outcome into page state.</para>
/// </summary>
[AllowAnonymous]
public sealed class RegisterModel : PageModel
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Initializes the page with the account service.
    /// </summary>
    /// <param name="accountService">The service performing registration.</param>
    public RegisterModel(IAccountService accountService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        _accountService = accountService;
    }

    /// <summary>
    /// Gets or sets the bound registration form input.
    /// </summary>
    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    /// <summary>
    /// Renders the registration form.
    /// </summary>
    public void OnGet()
    {
    }

    /// <summary>
    /// Processes the registration submission.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect to login on success, or the redisplayed form on failure.</returns>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        RegisterAccountRequest request = new(Input.Email, Input.DisplayName, Input.Password);
        Result<Guid> result = await _accountService.RegisterAsync(request, cancellationToken);
        if (result.IsSuccess)
        {
            TempData["StatusMessage"] = "Your account was created. Please sign in.";
            return RedirectToPage("/Account/Login");
        }

        ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
        return Page();
    }

    /// <summary>
    /// Carries the registration form fields with client-side validation attributes.
    /// </summary>
    public sealed class RegisterInputModel
    {
        /// <summary>
        /// Gets or sets the account email used as the username.
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name shown across the forum.
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 1)]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the chosen password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password confirmation, which must match <see cref="Password"/>.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
