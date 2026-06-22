using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ForumGuard.Application.Comments;
using ForumGuard.Application.Comments.Interfaces;
using ForumGuard.Application.Common;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages.Threads;

/// <summary>
/// Displays a thread's publicly visible comments and accepts comment submissions (SDD-FORUM-001).
/// <para>Reading is anonymous; submitting requires an authenticated active user and is delegated to
/// <see cref="ICommentSubmissionService"/>. The submitter sees a clear awaiting-review or published notice.</para>
/// </summary>
[AllowAnonymous]
public sealed class DetailsModel : PageModel
{
    private readonly IForumReadService _readService;
    private readonly ICommentSubmissionService _submissionService;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes the page with the read, submission, and Identity services.
    /// </summary>
    /// <param name="readService">The service that loads the thread detail view.</param>
    /// <param name="submissionService">The service that submits comments.</param>
    /// <param name="userManager">The Identity user manager used to resolve the author's active state.</param>
    public DetailsModel(
        IForumReadService readService,
        ICommentSubmissionService submissionService,
        UserManager<ApplicationUser> userManager)
    {
        ArgumentNullException.ThrowIfNull(readService);
        ArgumentNullException.ThrowIfNull(submissionService);
        ArgumentNullException.ThrowIfNull(userManager);
        _readService = readService;
        _submissionService = submissionService;
        _userManager = userManager;
    }

    /// <summary>
    /// Gets the thread and its visible comments.
    /// </summary>
    public ThreadDetailView? Thread { get; private set; }

    /// <summary>
    /// Gets or sets the bound comment body.
    /// </summary>
    [BindProperty]
    [Required]
    [StringLength(4000, MinimumLength = 1)]
    [Display(Name = "Your comment")]
    public string CommentBody { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the success notice shown to the submitter after a decision.
    /// </summary>
    [TempData]
    public string? SubmissionNotice { get; set; }

    /// <summary>
    /// Loads the thread detail.
    /// </summary>
    /// <param name="id">The thread identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The page, or a not-found result when the thread does not exist.</returns>
    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        Thread = await _readService.GetThreadDetailAsync(id, cancellationToken);
        return Thread is null ? NotFound() : Page();
    }

    /// <summary>
    /// Submits a comment to the thread on behalf of the active signed-in user.
    /// </summary>
    /// <param name="id">The thread identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect on success, or the redisplayed thread with errors.</returns>
    public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return await ReloadAsync(id, cancellationToken);
        }

        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        Result<SubmitCommentResult> result = await SubmitAsync(id, user, cancellationToken);
        if (result.IsSuccess)
        {
            SubmissionNotice = result.Value!.Status == CommentStatus.Published
                ? "Your comment was published."
                : "Your comment was submitted and is awaiting review.";
            return RedirectToPage(new { id });
        }

        ModelState.AddModelError(nameof(CommentBody), result.Error ?? "Could not submit your comment.");
        return await ReloadAsync(id, cancellationToken);
    }

    private Task<Result<SubmitCommentResult>> SubmitAsync(Guid id, ApplicationUser user, CancellationToken cancellationToken)
    {
        Guid signedInUserId = ResolveUserId();
        SubmitCommentRequest request = new(id, user.Id, signedInUserId, CommentBody, user.IsActive);
        return _submissionService.SubmitCommentAsync(request, cancellationToken);
    }

    private async Task<IActionResult> ReloadAsync(Guid id, CancellationToken cancellationToken)
    {
        Thread = await _readService.GetThreadDetailAsync(id, cancellationToken);
        return Thread is null ? NotFound() : Page();
    }

    private Guid ResolveUserId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}
