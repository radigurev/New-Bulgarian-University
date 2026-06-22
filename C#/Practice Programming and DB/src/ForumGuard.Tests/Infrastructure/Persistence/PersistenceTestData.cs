using ForumGuard.Domain.Entities;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;

namespace ForumGuard.Tests.Infrastructure.Persistence;

/// <summary>
/// Provides helpers for seeding the parent rows (users, threads) that ForumGuard foreign keys
/// require before comments and moderation decisions can be persisted in integration tests.
/// </summary>
public static class PersistenceTestData
{
    private static readonly DateTime SeedInstant = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Adds an <see cref="ApplicationUser"/> with the supplied activation flag to the context.
    /// </summary>
    /// <param name="context">The context to seed.</param>
    /// <param name="displayName">The display name for the user.</param>
    /// <param name="isActive">Whether the account is active.</param>
    /// <returns>The created, tracked user.</returns>
    public static ApplicationUser AddUser(ForumGuardDbContext context, string displayName, bool isActive = true)
    {
        ArgumentNullException.ThrowIfNull(context);

        Guid id = Guid.NewGuid();
        ApplicationUser user = new()
        {
            Id = id,
            UserName = $"{displayName}-{id:N}",
            NormalizedUserName = $"{displayName}-{id:N}".ToUpperInvariant(),
            Email = $"{id:N}@example.test",
            NormalizedEmail = $"{id:N}@EXAMPLE.TEST",
            DisplayName = displayName,
            IsActive = isActive,
            CreatedAtUtc = SeedInstant,
            SecurityStamp = Guid.NewGuid().ToString("N")
        };

        context.Users.Add(user);
        return user;
    }

    /// <summary>
    /// Adds a <see cref="ForumThread"/> owned by the supplied creator to the context.
    /// </summary>
    /// <param name="context">The context to seed.</param>
    /// <param name="createdById">The identifier of the creating user.</param>
    /// <param name="title">The thread title.</param>
    /// <returns>The created, tracked thread.</returns>
    public static ForumThread AddThread(ForumGuardDbContext context, Guid createdById, string title = "Test thread")
    {
        ArgumentNullException.ThrowIfNull(context);

        ForumThread thread = new(title, createdById, SeedInstant);
        context.ForumThreads.Add(thread);
        return thread;
    }
}
