using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Comments;
using ForumGuard.Application.Comments.Interfaces;
using ForumGuard.Application.Common;
using ForumGuard.Domain.Authorization;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Web.Composition.Interfaces;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Seeds the three roles, one ready-to-use development account per role (Administrator, Moderator,
/// User) plus a few community comment authors, and a populated set of sample threads and comments
/// (a realistic mix of published and flagged) idempotently at startup (SDD-FORUM-004, SDD-FORUM-011 V-1).
/// <para>The default account credentials are a development convenience for a zero-setup demo and are
/// logged with a warning to change them; they must never be used in a deployed environment.</para>
/// </summary>
public sealed class DataSeeder : IDataSeeder
{
    /// <summary>
    /// The email of the default development Administrator account.
    /// </summary>
    public const string DefaultAdministratorEmail = "admin@forumguard.local";

    /// <summary>
    /// The development default password for the seeded Administrator; intended to be changed.
    /// </summary>
    public const string DefaultAdministratorPassword = "Admin#12345";

    /// <summary>
    /// The email of the default development Moderator account.
    /// </summary>
    public const string DefaultModeratorEmail = "moderator@forumguard.local";

    /// <summary>
    /// The development default password for the seeded Moderator; intended to be changed.
    /// </summary>
    public const string DefaultModeratorPassword = "Mod#12345";

    /// <summary>
    /// The email of the default development standard User account.
    /// </summary>
    public const string DefaultUserEmail = "user@forumguard.local";

    /// <summary>
    /// The development default password for the seeded standard User; intended to be changed.
    /// </summary>
    public const string DefaultUserPassword = "User#12345";

    /// <summary>
    /// The development default password shared by the seeded community comment authors.
    /// </summary>
    public const string CommunityAuthorPassword = "Member#12345";

    private static readonly string[] RoleNames = [ForumRoles.User, ForumRoles.Moderator, ForumRoles.Administrator];

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ForumGuardDbContext _context;
    private readonly IThreadCreationService _threadCreationService;
    private readonly ICommentSubmissionService _commentSubmissionService;
    private readonly IClock _clock;
    private readonly ILogger<DataSeeder> _logger;

    /// <summary>
    /// Initializes the seeder with its Identity, persistence, and use-case dependencies.
    /// </summary>
    /// <param name="roleManager">The Identity role manager.</param>
    /// <param name="userManager">The Identity user manager.</param>
    /// <param name="context">The database context used to test for existing sample content.</param>
    /// <param name="threadCreationService">The service used to create the sample thread.</param>
    /// <param name="commentSubmissionService">The service used to create sample comments via analysis.</param>
    /// <param name="clock">The UTC time source for account creation timestamps.</param>
    /// <param name="logger">The logger for seeding outcomes and the dev-credentials warning.</param>
    public DataSeeder(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<ApplicationUser> userManager,
        ForumGuardDbContext context,
        IThreadCreationService threadCreationService,
        ICommentSubmissionService commentSubmissionService,
        IClock clock,
        ILogger<DataSeeder> logger)
    {
        ArgumentNullException.ThrowIfNull(roleManager);
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(threadCreationService);
        ArgumentNullException.ThrowIfNull(commentSubmissionService);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(logger);
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _threadCreationService = threadCreationService;
        _commentSubmissionService = commentSubmissionService;
        _clock = clock;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureRolesAsync().ConfigureAwait(false);

        Dictionary<string, ApplicationUser> users = new(StringComparer.Ordinal);
        users["admin"] = await EnsureUserAsync(DefaultAdministratorEmail, "Site Administrator", DefaultAdministratorPassword, [ForumRoles.Administrator, ForumRoles.User]).ConfigureAwait(false);
        users["moderator"] = await EnsureUserAsync(DefaultModeratorEmail, "Demo Moderator", DefaultModeratorPassword, [ForumRoles.Moderator, ForumRoles.User]).ConfigureAwait(false);
        users["user"] = await EnsureUserAsync(DefaultUserEmail, "Demo User", DefaultUserPassword, [ForumRoles.User]).ConfigureAwait(false);
        users["mira"] = await EnsureUserAsync("mira@forumguard.local", "Mira Petrova", CommunityAuthorPassword, [ForumRoles.User]).ConfigureAwait(false);
        users["theo"] = await EnsureUserAsync("theo@forumguard.local", "Theo Dimitrov", CommunityAuthorPassword, [ForumRoles.User]).ConfigureAwait(false);
        users["sam"] = await EnsureUserAsync("sam@forumguard.local", "Sam Ivanov", CommunityAuthorPassword, [ForumRoles.User]).ConfigureAwait(false);

        await EnsureSampleContentAsync(users, cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureRolesAsync()
    {
        foreach (string roleName in RoleNames)
        {
            if (!await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).ConfigureAwait(false);
            }
        }
    }

    private async Task<ApplicationUser> EnsureUserAsync(string email, string displayName, string password, string[] roles)
    {
        ApplicationUser? existing = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
        if (existing is not null)
        {
            return existing;
        }

        ApplicationUser user = new()
        {
            UserName = email,
            Email = email,
            DisplayName = displayName,
            IsActive = true,
            CreatedAtUtc = _clock.UtcNow,
            EmailConfirmed = true
        };

        IdentityResult creation = await _userManager.CreateAsync(user, password).ConfigureAwait(false);
        if (!creation.Succeeded)
        {
            string errors = string.Join(" ", creation.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to seed the default account '{email}': {errors}");
        }

        await _userManager.AddToRolesAsync(user, roles).ConfigureAwait(false);
        _logger.LogWarning(
            "Seeded default development account {Email} ({Roles}) with a documented default password. Change it before deployment.",
            email, string.Join("+", roles));

        return user;
    }

    private async Task EnsureSampleContentAsync(IReadOnlyDictionary<string, ApplicationUser> users, CancellationToken cancellationToken)
    {
        if (await _context.ForumThreads.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        await SeedThreadAsync(users, "admin", "Welcome to ForumGuard",
        [
            ("admin", "Glad to have you here. Keep it civil and constructive."),
            ("mira", "Thanks for setting this up — looking forward to the discussions."),
            ("theo", "you are all a bunch of idiots and this place is trash"),
            ("sam", "Happy to be here, the layout is clean and easy to read.")
        ], cancellationToken).ConfigureAwait(false);

        await SeedThreadAsync(users, "user", "Which editor do you use for C#?",
        [
            ("theo", "Rider for me — the refactoring tools are worth it."),
            ("mira", "VS Code with the C# Dev Kit, fast and lightweight."),
            ("sam", "honestly anyone still using Notepad is a complete moron"),
            ("admin", "Visual Studio for debugging, VS Code for quick edits.")
        ], cancellationToken).ConfigureAwait(false);

        await SeedThreadAsync(users, "moderator", "Tips for good code reviews",
        [
            ("mira", "Keep pull requests small so they are easy to review."),
            ("sam", "this entire thread is stupid, just approve everything and move on"),
            ("theo", "Run the automated checks first, save people for the design feedback."),
            ("user", "Be kind in the comments — review the code, not the person.")
        ], cancellationToken).ConfigureAwait(false);

        await SeedThreadAsync(users, "user", "Weekend project ideas",
        [
            ("theo", "Thinking of a small CLI tool to rename my photo files."),
            ("sam", "what a pathetic list, none of these are real projects"),
            ("mira", "I am building a tiny game in Godot — surprisingly fun.")
        ], cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedThreadAsync(
        IReadOnlyDictionary<string, ApplicationUser> users,
        string creatorKey,
        string title,
        (string Author, string Body)[] comments,
        CancellationToken cancellationToken)
    {
        Result<Guid> thread = await _threadCreationService
            .CreateThreadAsync(title, users[creatorKey].Id, cancellationToken)
            .ConfigureAwait(false);
        if (!thread.IsSuccess)
        {
            return;
        }

        foreach ((string author, string body) in comments)
        {
            await SubmitSampleCommentAsync(thread.Value, users[author].Id, body, cancellationToken).ConfigureAwait(false);
        }

        _logger.LogInformation("Seeded sample thread '{Title}' with {Count} comments.", title, comments.Length);
    }

    private Task SubmitSampleCommentAsync(Guid threadId, Guid authorId, string body, CancellationToken cancellationToken)
    {
        SubmitCommentRequest request = new(threadId, authorId, authorId, body, IsAuthorActive: true);
        return _commentSubmissionService.SubmitCommentAsync(request, cancellationToken);
    }
}
