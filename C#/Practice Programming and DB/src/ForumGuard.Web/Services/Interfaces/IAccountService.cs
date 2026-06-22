using ForumGuard.Application.Common;
using ForumGuard.Web.Services.Dtos;

namespace ForumGuard.Web.Services.Interfaces;

/// <summary>
/// Defines the Web-layer account use cases over ASP.NET Core Identity (SDD-FORUM-003).
/// <para>Covers self-registration with the default User role, sign-in with the IsActive gate,
/// sign-out, and Administrator activation/deactivation with the self-deactivation guard.</para>
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Registers a new active account in the User role.
    /// </summary>
    /// <param name="request">The registration input.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result with the new user id, or a typed failure (conflict/validation).</returns>
    Task<Result<Guid>> RegisterAsync(RegisterAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs a user in, enforcing the IsActive precondition.
    /// </summary>
    /// <param name="request">The sign-in credentials.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result, or a typed failure (validation for bad credentials, forbidden for inactive).</returns>
    Task<Result<Guid>> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signs the current user out.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous sign-out.</returns>
    Task SignOutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all accounts with their active state and role membership for Administrator management.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The account rows ordered by display name.</returns>
    Task<IReadOnlyList<AccountListItem>> ListAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates or deactivates a target account, enforcing the self-deactivation guard.
    /// </summary>
    /// <param name="request">The activation request.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A success result with the resulting active state, or a typed failure.</returns>
    Task<Result<bool>> SetActiveAsync(SetAccountActiveRequest request, CancellationToken cancellationToken = default);
}
