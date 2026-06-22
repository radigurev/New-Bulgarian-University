using ForumGuard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Tests.Infrastructure.Persistence;

/// <summary>
/// Base fixture for EF Core integration tests against a dedicated SQL Server LocalDB database.
/// <para>See SDD-FORUM-022 (B-29). Creates and migrates a per-fixture <c>ForumGuardDbTest_*</c>
/// database in <see cref="OneTimeSetUp"/> and drops it in <see cref="OneTimeTearDown"/> so the
/// production <c>ForumGuardDb</c> is never touched.</para>
/// </summary>
public abstract class ForumGuardDbContextFixture
{
    private const string ServerConnectionTemplate =
        "Server=(localdb)\\MSSQLLocalDB;Database={0};Trusted_Connection=True;" +
        "TrustServerCertificate=True;MultipleActiveResultSets=true";

    /// <summary>
    /// Gets the connection string targeting the dedicated test database.
    /// </summary>
    protected string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    /// Creates and migrates the dedicated test database before any test in the fixture runs.
    /// </summary>
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        string databaseName = $"ForumGuardDbTest_{Guid.NewGuid():N}";
        ConnectionString = string.Format(ServerConnectionTemplate, databaseName);

        await using ForumGuardDbContext context = CreateContext();
        await context.Database.MigrateAsync(CancellationToken.None);
    }

    /// <summary>
    /// Clears all ForumGuard rows before each test so tests in a shared-database fixture do not
    /// observe each other's data. The migrated schema is preserved.
    /// </summary>
    [SetUp]
    public async Task ResetData()
    {
        await using ForumGuardDbContext context = CreateContext();
        await context.Database.ExecuteSqlRawAsync(
            "DELETE FROM ModerationDecisions; DELETE FROM Comments; DELETE FROM ForumThreads; " +
            "DELETE FROM AspNetUserRoles; DELETE FROM AspNetUsers;",
            CancellationToken.None);
    }

    /// <summary>
    /// Drops the dedicated test database after every test in the fixture completes.
    /// </summary>
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await using ForumGuardDbContext context = CreateContext();
        await context.Database.EnsureDeletedAsync(CancellationToken.None);
    }

    /// <summary>
    /// Creates a fresh <see cref="ForumGuardDbContext"/> bound to the dedicated test database.
    /// </summary>
    /// <returns>A new database context.</returns>
    protected ForumGuardDbContext CreateContext()
    {
        DbContextOptions<ForumGuardDbContext> options = new DbContextOptionsBuilder<ForumGuardDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new ForumGuardDbContext(options);
    }
}
