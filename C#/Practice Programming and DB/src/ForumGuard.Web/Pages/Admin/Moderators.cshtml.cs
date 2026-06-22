using ForumGuard.Application.Common;
using ForumGuard.Web.Authorization;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Admin;

/// <summary>
/// Lists accounts and lets an Administrator grant or revoke the Moderator role (SDD-FORUM-004).
/// <para>Gated by <see cref="ForumPolicies.CanManageModerators"/>. Delegates to
/// <see cref="IModeratorRoleService"/>; redundant grant/revoke is reported as an informational no-op.</para>
/// </summary>
[Authorize(Policy = ForumPolicies.CanManageModerators)]
public sealed class ModeratorsModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly IModeratorRoleService _moderatorRoleService;

    /// <summary>
    /// Initializes the page with the account and moderator-role services.
    /// </summary>
    /// <param name="accountService">The service that lists accounts.</param>
    /// <param name="moderatorRoleService">The service that grants and revokes the Moderator role.</param>
    public ModeratorsModel(IAccountService accountService, IModeratorRoleService moderatorRoleService)
    {
        ArgumentNullException.ThrowIfNull(accountService);
        ArgumentNullException.ThrowIfNull(moderatorRoleService);
        _accountService = accountService;
        _moderatorRoleService = moderatorRoleService;
    }

    /// <summary>
    /// Gets the accounts displayed in the management list.
    /// </summary>
    public IReadOnlyList<AccountListItem> Accounts { get; private set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the account whose Moderator role is being changed.
    /// </summary>
    [BindProperty]
    public Guid TargetUserId { get; set; }

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
    /// Grants the Moderator role to the target account.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect back to the management list.</returns>
    public async Task<IActionResult> OnPostGrantAsync(CancellationToken cancellationToken)
    {
        Result<ModeratorRoleChangeOutcome> result = await _moderatorRoleService.GrantModeratorAsync(TargetUserId, cancellationToken);
        ApplyOutcome(result, "User is now a Moderator.", "User is already a Moderator.");
        return RedirectToPage();
    }

    /// <summary>
    /// Revokes the Moderator role from the target account.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect back to the management list.</returns>
    public async Task<IActionResult> OnPostRevokeAsync(CancellationToken cancellationToken)
    {
        Result<ModeratorRoleChangeOutcome> result = await _moderatorRoleService.RevokeModeratorAsync(TargetUserId, cancellationToken);
        ApplyOutcome(result, "Moderator role removed.", "User was not a Moderator.");
        return RedirectToPage();
    }

    private void ApplyOutcome(Result<ModeratorRoleChangeOutcome> result, string changedMessage, string noOpMessage)
    {
        if (!result.IsSuccess)
        {
            ErrorMessage = result.Error ?? "Could not update the Moderator role.";
            return;
        }

        StatusMessage = result.Value == ModeratorRoleChangeOutcome.NoOp ? noOpMessage : changedMessage;
    }
}
