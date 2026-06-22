using System.Security.Claims;
using ForumGuard.Application.Common;
using ForumGuard.Application.Moderation.Dtos;
using ForumGuard.Application.Moderation.Interfaces;
using ForumGuard.Domain.Enums;
using ForumGuard.Web.Authorization;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Moderation;

/// <summary>
/// Lists flagged comments and applies Approve/Reject decisions (SDD-FORUM-002).
/// <para>Gated by <see cref="ForumPolicies.CanModerateComments"/> (Administrators excluded). Listing
/// and decisions are delegated to <see cref="IForumReadService"/> and <see cref="IModerationService"/>.</para>
/// </summary>
[Authorize(Policy = ForumPolicies.CanModerateComments)]
public sealed class QueueModel : PageModel
{
    private readonly IForumReadService _readService;
    private readonly IModerationService _moderationService;

    /// <summary>
    /// Initializes the page with the read and moderation services.
    /// </summary>
    /// <param name="readService">The service that loads queue rows with author names.</param>
    /// <param name="moderationService">The service that applies decisions.</param>
    public QueueModel(IForumReadService readService, IModerationService moderationService)
    {
        ArgumentNullException.ThrowIfNull(readService);
        ArgumentNullException.ThrowIfNull(moderationService);
        _readService = readService;
        _moderationService = moderationService;
    }

    /// <summary>
    /// Gets the flagged comments awaiting review.
    /// </summary>
    public IReadOnlyList<QueueRowView> Rows { get; private set; } = [];

    /// <summary>
    /// Gets or sets the identifier of the comment being decided.
    /// </summary>
    [BindProperty]
    public Guid CommentId { get; set; }

    /// <summary>
    /// Gets or sets an informational notice shown after a decision.
    /// </summary>
    [TempData]
    public string? DecisionNotice { get; set; }

    /// <summary>
    /// Gets or sets an error message shown after a failed decision.
    /// </summary>
    [TempData]
    public string? DecisionError { get; set; }

    /// <summary>
    /// Loads the moderation queue.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous load.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Rows = await _readService.GetQueueRowsAsync(cancellationToken);
    }

    /// <summary>
    /// Approves the selected flagged comment.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect back to the queue.</returns>
    public Task<IActionResult> OnPostApproveAsync(CancellationToken cancellationToken) =>
        DecideAsync(ModerationOutcome.Approved, cancellationToken);

    /// <summary>
    /// Rejects the selected flagged comment.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect back to the queue.</returns>
    public Task<IActionResult> OnPostRejectAsync(CancellationToken cancellationToken) =>
        DecideAsync(ModerationOutcome.Rejected, cancellationToken);

    private async Task<IActionResult> DecideAsync(ModerationOutcome outcome, CancellationToken cancellationToken)
    {
        ModerationDecisionRequest request = new(CommentId, ResolveModeratorId(), outcome);
        Result<ModerationDecisionResult> result = await _moderationService.DecideAsync(request, cancellationToken);
        if (result.IsSuccess)
        {
            DecisionNotice = outcome == ModerationOutcome.Approved
                ? "The comment was approved and published."
                : "The comment was rejected and stays hidden.";
        }
        else
        {
            DecisionError = result.Error ?? "Could not save your decision. Please retry.";
        }

        return RedirectToPage();
    }

    private Guid ResolveModeratorId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}
