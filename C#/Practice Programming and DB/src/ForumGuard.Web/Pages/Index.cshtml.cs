using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ForumGuard.Application.Common;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ForumGuard.Web.Pages;

/// <summary>
/// Lists forum threads newest-first and lets an authenticated user create a thread (SDD-FORUM-001 context).
/// <para>Delegates listing to <see cref="IForumReadService"/> and creation to
/// <see cref="IThreadCreationService"/>; the handler stays free of domain logic.</para>
/// </summary>
[AllowAnonymous]
public sealed class IndexModel : PageModel
{
    private readonly IForumReadService _readService;
    private readonly IThreadCreationService _threadCreationService;

    /// <summary>
    /// Initializes the page with the read and creation services.
    /// </summary>
    /// <param name="readService">The service that lists threads.</param>
    /// <param name="threadCreationService">The service that creates threads.</param>
    public IndexModel(IForumReadService readService, IThreadCreationService threadCreationService)
    {
        ArgumentNullException.ThrowIfNull(readService);
        ArgumentNullException.ThrowIfNull(threadCreationService);
        _readService = readService;
        _threadCreationService = threadCreationService;
    }

    /// <summary>
    /// Gets the threads displayed in the listing.
    /// </summary>
    public IReadOnlyList<ThreadListItem> Threads { get; private set; } = [];

    /// <summary>
    /// Gets or sets the bound new-thread title.
    /// </summary>
    [BindProperty]
    [Required]
    [StringLength(200, MinimumLength = 1)]
    [Display(Name = "Thread title")]
    public string NewThreadTitle { get; set; } = string.Empty;

    /// <summary>
    /// Loads the thread listing.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous load.</returns>
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Threads = await _readService.ListThreadsAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a new thread authored by the signed-in user.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A redirect to the new thread on success, or the redisplayed listing on failure.</returns>
    public async Task<IActionResult> OnPostCreateThreadAsync(CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            Threads = await _readService.ListThreadsAsync(cancellationToken);
            return Page();
        }

        Guid userId = ResolveUserId();
        Result<Guid> result = await _threadCreationService.CreateThreadAsync(NewThreadTitle, userId, cancellationToken);
        if (result.IsSuccess)
        {
            return RedirectToPage("/Threads/Details", new { id = result.Value });
        }

        ModelState.AddModelError(nameof(NewThreadTitle), result.Error ?? "Could not create the thread.");
        Threads = await _readService.ListThreadsAsync(cancellationToken);
        return Page();
    }

    private Guid ResolveUserId()
    {
        string? value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}
