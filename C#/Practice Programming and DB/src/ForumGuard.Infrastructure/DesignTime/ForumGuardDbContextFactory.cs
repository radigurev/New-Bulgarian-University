using ForumGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ForumGuard.Infrastructure.DesignTime;

/// <summary>
/// Creates a <see cref="ForumGuardDbContext"/> for design-time tooling such as
/// <c>dotnet ef migrations</c> and <c>dotnet ef database update</c>, without requiring a host.
/// <para>See SDD-FORUM-022. Uses the local SQL Server LocalDB development connection string.</para>
/// </summary>
public sealed class ForumGuardDbContextFactory : IDesignTimeDbContextFactory<ForumGuardDbContext>
{
    private const string DesignTimeConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=ForumGuardDb;Trusted_Connection=True;" +
        "TrustServerCertificate=True;MultipleActiveResultSets=true";

    /// <inheritdoc />
    public ForumGuardDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<ForumGuardDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(
            DesignTimeConnectionString,
            sqlServerOptions => sqlServerOptions.MigrationsAssembly(
                typeof(ForumGuardDbContext).Assembly.FullName));

        return new ForumGuardDbContext(optionsBuilder.Options);
    }
}
