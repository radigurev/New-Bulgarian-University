namespace ForumGuard.Domain.Authorization;

/// <summary>
/// Provides the authoritative role-name string constants for ForumGuard (SDD-FORUM-011 B-1, V-1).
/// <para>This is the single source of truth for the three Identity roles; no handler, policy, or
/// workspace may hard-code a role literal independently. The values mirror the Identity seed data.</para>
/// </summary>
public static class ForumRoles
{
    /// <summary>
    /// The base role assigned to every registered account.
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// The additive role permitting comment moderation.
    /// </summary>
    public const string Moderator = "Moderator";

    /// <summary>
    /// The additive role permitting moderator-management and account-management.
    /// </summary>
    public const string Administrator = "Administrator";
}
