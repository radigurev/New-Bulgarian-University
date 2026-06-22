namespace MeepleNight.Web.Infrastructure.Seeding.Interfaces;

/// <summary>
/// Seeds illustrative development data (extra accounts, game nights, invitations,
/// sessions) on top of the bootstrap admin so the UI has realistic content to render.
/// Implementations must be idempotent and must only execute in development.
/// </summary>
public interface IDevDataSeeder
{
    /// <summary>Runs the development data seed once. Safe to call repeatedly.</summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}
