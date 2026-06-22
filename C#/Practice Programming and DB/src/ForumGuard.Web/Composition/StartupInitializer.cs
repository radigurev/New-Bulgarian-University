using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Web.Composition.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Applies pending EF Core migrations and runs the idempotent <see cref="IDataSeeder"/> in Development.
/// <para>Executed once at startup within a scoped service provider; safe to re-run.</para>
/// </summary>
public static class StartupInitializer
{
    /// <summary>
    /// Migrates the database and seeds development data using a fresh service scope.
    /// </summary>
    /// <param name="app">The built web application.</param>
    /// <returns>A task representing the asynchronous initialization.</returns>
    public static async Task InitializeDevelopmentAsync(WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
        ForumGuardDbContext context = scope.ServiceProvider.GetRequiredService<ForumGuardDbContext>();
        await context.Database.MigrateAsync().ConfigureAwait(false);

        IDataSeeder seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync().ConfigureAwait(false);
    }
}
