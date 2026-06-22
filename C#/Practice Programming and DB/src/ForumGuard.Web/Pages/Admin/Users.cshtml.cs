using System.Security.Claims;
using ForumGuard.Application.Common;
using ForumGuard.Web.Authorization;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Admin;

/// <summary>
/// Lists accounts and lets an Administrator activate or deactivate them (SDD-FORUM-003).
/// <para>Gated by <see cref="ForumPolicies.CanManageAccounts"/>. Delegates to
/// <see cref="IAccountService"/>; the self-deactivation guard is enforced by the service.</para>
/// </summary>
[Authorize(Policy = ForumPolicies.CanManageAccounts)]
public sealed class UsersModel : PageModel
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Initializes the page with the account service.
    /// </summary>
    /// <param name="accountService">The service that lists and toggles accounts.</param>
    public UsersModel(IAccountService accountService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        _accountService = accountService;
    }

    /// <summary>
    /// Gets the accounts displayed in the management list.
    /// </summary>
    public IReadOnlyList<AccountListItem> Accounts { get; private set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the account being toggled.
    /// </summary>
    [BindProperty]
    public Guid TargetUserId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the toggle activates the target.
    /// </summary>
    [BindProperty]
    public bool Activate { get; set; }

    /// <summary>
    /// Gets or sets an informational notice shown after a change.
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Gets or sets an error message shown after a failed change.
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Loads the account list.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous load.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Accounts = await _accountService.ListAccountsAsync(cancellationToken);
    }

    /// <summary>
    /// Applies the activate/deactivate change to the target account.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect back to the account list.</returns>
    public async Task<IActionResult> OnPostToggleAsync(CancellationToken cancellationToken)
    {
        SetAccountActiveRequest request = new(TargetUserId, ResolveAdministratorId(), Activate);
        Result<bool> result = await _accountService.SetActiveAsync(request, cancellationToken);
        if (result.IsSuccess)
        {
            StatusMessage = result.Value ? "Account activated." : "Account deactivated.";
        }
        else
        {
            ErrorMessage = result.Error ?? "Could not update the account.";
        }

        return RedirectToPage();
    }

    private Guid ResolveAdministratorId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}
