using ForumGuard.Application.Abstractions;
using ForumGuard.Application.Common;
using ForumGuard.Domain.Authorization;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Web.Services.Dtos;
using ForumGuard.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ForumGuard.Web.Services;

/// <summary>
/// Implements the Web-layer account use cases over ASP.NET Core Identity (SDD-FORUM-003).
/// <para>Delegates persistence to <see cref="UserManager{TUser}"/> and cookie sign-in to
/// <see cref="SignInManager{TUser}"/>; enforces the IsActive gate and self-deactivation guard.</para>
/// </summary>
public sealed class AccountService : IAccountService
{
    private const int MaxDisplayNameLength = 128;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IClock _clock;
    private readonly ILogger<AccountService> _logger;

    /// <summary>
    /// Initializes the account service with the Identity managers, clock, and logger.
    /// </summary>
    /// <param name="userManager">The Identity user store manager.</param>
    /// <param name="signInManager">The Identity cookie sign-in manager.</param>
    /// <param name="clock">The UTC time source for creation timestamps.</param>
    /// <param name="logger">The logger for sign-in and registration outcomes.</param>
    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IClock clock,
        ILogger<AccountService> logger)
    {
        ArgumentNullException.ThrowIfNull(userManager);
        ArgumentNullException.ThrowIfNull(signInManager);
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(logger);
        _userManager = userManager;
        _signInManager = signInManager;
        _clock = clock;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> RegisterAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        Result<Guid>? validation = ValidateRegistration(request);
        if (validation is not null)
        {
            return validation;
        }

        ApplicationUser user = new()
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            DisplayName = request.DisplayName.Trim(),
            IsActive = true,
            CreatedAtUtc = _clock.UtcNow
        };

        IdentityResult creation = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
        if (!creation.Succeeded)
        {
            return MapRegistrationFailure(creation);
        }

        await _userManager.AddToRoleAsync(user, ForumRoles.User).ConfigureAwait(false);
        _logger.LogInformation("Registered account {UserId} in the User role.", user.Id);
        return Result<Guid>.Success(user.Id);
    }

    /// <inheritdoc />
    public async Task<Result<Guid>> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email.Trim()).ConfigureAwait(false);
        if (user is null)
        {
            return Result<Guid>.Validation("Invalid login attempt.");
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Sign-in blocked for deactivated account {UserId}.", user.Id);
            return Result<Guid>.Forbidden("This account is deactivated. Contact an administrator.");
        }

        return await TryPasswordSignInAsync(user, request).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _signInManager.SignOutAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AccountListItem>> ListAccountsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        List<ApplicationUser> users = _userManager.Users.OrderBy(user => user.DisplayName).ToList();
        List<AccountListItem> items = new(users.Count);
        foreach (ApplicationUser user in users)
        {
            items.Add(await MapAccountAsync(user).ConfigureAwait(false));
        }

        return items;
    }

    /// <inheritdoc />
    public async Task<Result<bool>> SetActiveAsync(SetAccountActiveRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (!request.Activate && request.TargetUserId == request.ActingAdministratorId)
        {
            return Result<bool>.Forbidden("You cannot deactivate your own account.");
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(request.TargetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
        {
            return Result<bool>.NotFound("The target account was not found.");
        }

        user.IsActive = request.Activate;
        IdentityResult update = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!update.Succeeded)
        {
            return Result<bool>.Conflict("Could not update the account. Please try again.");
        }

        return Result<bool>.Success(user.IsActive);
    }

    private async Task<Result<Guid>> TryPasswordSignInAsync(ApplicationUser user, SignInRequest request)
    {
        SignInResult result = await _signInManager
            .PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: false)
            .ConfigureAwait(false);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed sign-in attempt for account {UserId}.", user.Id);
            return Result<Guid>.Validation("Invalid login attempt.");
        }

        _logger.LogInformation("Account {UserId} signed in.", user.Id);
        return Result<Guid>.Success(user.Id);
    }

    private static Result<Guid>? ValidateRegistration(RegisterAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result<Guid>.Validation("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return Result<Guid>.Validation("Display name is required.");
        }

        if (request.DisplayName.Trim().Length > MaxDisplayNameLength)
        {
            return Result<Guid>.Validation($"Display name must not exceed {MaxDisplayNameLength} characters.");
        }

        return null;
    }

    private static Result<Guid> MapRegistrationFailure(IdentityResult creation)
    {
        if (creation.Errors.Any(error => error.Code.Contains("Duplicate", StringComparison.Ordinal)))
        {
            return Result<Guid>.Conflict("An account with this email already exists.");
        }

        string message = string.Join(" ", creation.Errors.Select(error => error.Description));
        return Result<Guid>.Validation(message);
    }

    private async Task<AccountListItem> MapAccountAsync(ApplicationUser user)
    {
        IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        return new AccountListItem(
            user.Id,
            user.Email ?? string.Empty,
            user.DisplayName,
            user.IsActive,
            roles.Contains(ForumRoles.Moderator),
            roles.Contains(ForumRoles.Administrator));
    }
}
