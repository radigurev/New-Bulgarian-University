using Microsoft.AspNetCore.Identity;

namespace ForumGuard.Infrastructure.Identity;

/// <summary>
/// Represents an authenticated ForumGuard account, extending the ASP.NET Core Identity user
/// with the moderation-domain audit and activation columns.
/// <para>Account deactivation is performed via <see cref="IsActive"/> (see SDD-FORUM-003), never by
/// deleting the row; author, creator and moderator relationships use restrict delete behavior
/// (see SDD-FORUM-022).</para>
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the display name shown for the user across the forum; required.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the account is active. Administrators toggle this
    /// flag to deactivate accounts instead of deleting them.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the UTC instant at which the account was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }
}
