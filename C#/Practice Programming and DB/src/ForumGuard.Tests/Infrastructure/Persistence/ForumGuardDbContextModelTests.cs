using ForumGuard.Domain.Entities;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ForumGuard.Tests.Infrastructure.Persistence;

/// <summary>
/// Integration tests verifying the <see cref="ForumGuardDbContext"/> model (SDD-FORUM-022):
/// Identity and ForumGuard entities are registered, the Fluent API produces the expected table,
/// column and length mappings, and the initial migration applies cleanly and idempotently.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-022")]
[Category("Integration")]
public sealed class ForumGuardDbContextModelTests : ForumGuardDbContextFixture
{
    [Test]
    public void OnModelCreating_BuildsModel_RegistersIdentityAndForumGuardEntities()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();

        // Act
        IEntityType? userType = context.Model.FindEntityType(typeof(ApplicationUser));
        IEntityType? roleType = context.Model.FindEntityType(typeof(IdentityRole<Guid>));
        IEntityType? threadType = context.Model.FindEntityType(typeof(ForumThread));
        IEntityType? commentType = context.Model.FindEntityType(typeof(Comment));
        IEntityType? decisionType = context.Model.FindEntityType(typeof(ModerationDecision));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(userType, Is.Not.Null);
            Assert.That(roleType, Is.Not.Null);
            Assert.That(threadType, Is.Not.Null);
            Assert.That(commentType, Is.Not.Null);
            Assert.That(decisionType, Is.Not.Null);
        });
    }

    [Test]
    public void Model_ForumGuardTables_UseExplicitPluralNamesWithoutPrefix()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();

        // Act
        string? threadTable = context.Model.FindEntityType(typeof(ForumThread))!.GetTableName();
        string? commentTable = context.Model.FindEntityType(typeof(Comment))!.GetTableName();
        string? decisionTable = context.Model.FindEntityType(typeof(ModerationDecision))!.GetTableName();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(threadTable, Is.EqualTo("ForumThreads"));
            Assert.That(commentTable, Is.EqualTo("Comments"));
            Assert.That(decisionTable, Is.EqualTo("ModerationDecisions"));
        });
    }

    [Test]
    public void Model_CommentBody_IsRequiredWithMaxLength4000()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();
        IEntityType commentType = context.Model.FindEntityType(typeof(Comment))!;

        // Act
        IProperty bodyProperty = commentType.FindProperty(nameof(Comment.Body))!;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(bodyProperty.IsNullable, Is.False);
            Assert.That(bodyProperty.GetMaxLength(), Is.EqualTo(Comment.MaxBodyLength));
        });
    }

    [Test]
    public void Model_ForumThreadTitle_IsRequiredWithMaxLength200()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();
        IEntityType threadType = context.Model.FindEntityType(typeof(ForumThread))!;

        // Act
        IProperty titleProperty = threadType.FindProperty(nameof(ForumThread.Title))!;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(titleProperty.IsNullable, Is.False);
            Assert.That(titleProperty.GetMaxLength(), Is.EqualTo(ForumThread.MaxTitleLength));
        });
    }

    [Test]
    public void Model_ModerationDecisionReason_IsOptionalWithMaxLength500()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();
        IEntityType decisionType = context.Model.FindEntityType(typeof(ModerationDecision))!;

        // Act
        IProperty reasonProperty = decisionType.FindProperty(nameof(ModerationDecision.Reason))!;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(reasonProperty.IsNullable, Is.True);
            Assert.That(reasonProperty.GetMaxLength(), Is.EqualTo(ModerationDecision.MaxReasonLength));
        });
    }

    [Test]
    public void Model_CommentNullableTimestamps_HaveNoDatabaseDefault()
    {
        // Arrange
        using ForumGuardDbContext context = CreateContext();
        IEntityType commentType = context.Model.FindEntityType(typeof(Comment))!;

        // Act
        IProperty publishedAt = commentType.FindProperty(nameof(Comment.PublishedAtUtc))!;
        IProperty analyzedAt = commentType.FindProperty(nameof(Comment.AnalyzedAtUtc))!;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(publishedAt.IsNullable, Is.True);
            Assert.That(publishedAt.GetDefaultValueSql(), Is.Null);
            Assert.That(analyzedAt.IsNullable, Is.True);
            Assert.That(analyzedAt.GetDefaultValueSql(), Is.Null);
        });
    }

    [Test]
    public async Task InitialMigration_AppliesCleanly_AndIsIdempotentOnReRun()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();

        // Act
        IEnumerable<string> pendingBeforeReRun = await context.Database.GetPendingMigrationsAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(pendingBeforeReRun, Is.Empty);
            Assert.That(
                async () => await context.Database.MigrateAsync(CancellationToken.None),
                Throws.Nothing);
        });
    }
}
