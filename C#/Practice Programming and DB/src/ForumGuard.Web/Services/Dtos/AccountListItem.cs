namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Read model for one row of the Administrator account-management list (SDD-FORUM-003).
/// <para>Projects an <c>ApplicationUser</c> without exposing the ORM entity to the page.</para>
/// </summary>
/// <param name="UserId">The account identifier.</param>
/// <param name="Email">The account email.</param>
/// <param name="DisplayName">The display name shown across the forum.</param>
/// <param name="IsActive">Whether the account is currently active.</param>
/// <param name="IsModerator">Whether the account currently holds the Moderator role.</param>
/// <param name="IsAdministrator">Whether the account currently holds the Administrator role.</param>
public sealed record AccountListItem(
    Guid UserId,
    string Email,
    string DisplayName,
    bool IsActive,
    bool IsModerator,
    bool IsAdministrator);
