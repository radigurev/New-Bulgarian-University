namespace ForumGuard.Web.Composition.Interfaces;

/// <summary>
/// Defines the idempotent startup seeding of the three roles, the default Administrator, and sample
/// forum content (SDD-FORUM-004 §2, SDD-FORUM-011 V-1).
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Ensures the roles, default Administrator, and sample content exist; safe to run repeatedly.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous seeding.</returns>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
