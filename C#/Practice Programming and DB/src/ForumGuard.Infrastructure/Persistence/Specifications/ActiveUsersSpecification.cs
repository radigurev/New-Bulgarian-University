using ForumGuard.Infrastructure.Identity;

namespace ForumGuard.Infrastructure.Persistence.Specifications;

/// <summary>
/// Selects the active accounts: <see cref="ApplicationUser"/> rows whose
/// <see cref="ApplicationUser.IsActive"/> flag is <c>true</c>.
/// <para>See SDD-FORUM-022 (B-16).</para>
/// </summary>
public sealed class ActiveUsersSpecification : Specification<ApplicationUser>
{
    /// <summary>
    /// Initializes the specification matching only active users.
    /// </summary>
    public ActiveUsersSpecification()
        : base(user => user.IsActive)
    {
        ApplyOrderBy(user => user.DisplayName);
    }
}
