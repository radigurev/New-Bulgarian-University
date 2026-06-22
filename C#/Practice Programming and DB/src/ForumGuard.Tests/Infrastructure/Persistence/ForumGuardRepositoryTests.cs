using System.Data.Common;
using ForumGuard.Domain.Analysis;
using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Identity;
using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Infrastructure.Persistence.Repositories;
using ForumGuard.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ForumGuard.Tests.Infrastructure.Persistence;

/// <summary>
/// Integration tests for the ForumGuard EF Core persistence layer (SDD-FORUM-022) against a
/// dedicated SQL Server LocalDB database: round-trips, server-side defaults, schema naming,
/// constraint enforcement, unit-of-work semantics, delete behavior, concurrency, and the concrete
/// specifications.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-022")]
[Category("Integration")]
public sealed class ForumGuardRepositoryTests : ForumGuardDbContextFixture
{
    private static readonly DateTime BaseInstant = new(2026, 6, 21, 8, 0, 0, DateTimeKind.Utc);

    [Test]
    public async Task Comment_SaveAndReload_RoundTripsAllCanonicalColumns()
    {
        // Arrange
        await using ForumGuardDbContext writeContext = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(writeContext);
        Comment comment = new(threadId, authorId, "round-trip body", BaseInstant);
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, 0.25f), BaseInstant.AddMinutes(1));
        Guid commentId = comment.Id;

        CommentRepository writeRepository = new(writeContext);
        UnitOfWork writeUnitOfWork = new(writeContext);
        writeRepository.Add(comment);
        await writeUnitOfWork.SaveChangesAsync(CancellationToken.None);

        // Act
        await using ForumGuardDbContext readContext = CreateContext();
        Comment? reloaded = await new CommentRepository(readContext).GetByIdAsync(commentId, CancellationToken.None);

        // Assert
        Assert.That(reloaded, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(reloaded!.ThreadId, Is.EqualTo(threadId));
            Assert.That(reloaded.AuthorId, Is.EqualTo(authorId));
            Assert.That(reloaded.Body, Is.EqualTo("round-trip body"));
            Assert.That(reloaded.Status, Is.EqualTo(CommentStatus.Published));
            Assert.That(reloaded.AnalysisLabel, Is.EqualTo(ToxicityLabel.Clean));
            Assert.That(reloaded.AnalysisScore, Is.EqualTo(0.25f));
            Assert.That(reloaded.AnalyzedAtUtc, Is.EqualTo(BaseInstant.AddMinutes(1)));
            Assert.That(reloaded.PublishedAtUtc, Is.EqualTo(BaseInstant.AddMinutes(1)));
        });
    }

    [Test]
    public async Task GuidPrimaryKeys_OnInsert_PopulatedByNewSequentialIdDefault()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser user = PersistenceTestData.AddUser(context, "creator");
        await context.SaveChangesAsync(CancellationToken.None);
        string insertSql =
            "INSERT INTO ForumThreads (Title, CreatedById) " +
            $"OUTPUT INSERTED.Id VALUES ('default-key-thread', '{user.Id}')";

        // Act
        Guid serverGeneratedId = await ScalarGuidAsync(context, insertSql);

        // Assert
        Assert.That(serverGeneratedId, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task GuidPrimaryKeyDefaults_AreNewSequentialIdNotNewId()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        const string definitionSql =
            "SELECT definition FROM sys.default_constraints dc " +
            "JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id " +
            "JOIN sys.tables t ON t.object_id = dc.parent_object_id " +
            "WHERE t.name = 'Comments' AND c.name = 'Id'";

        // Act
        IReadOnlyList<string> definitions = await QueryNamesAsync(context, definitionSql);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(definitions, Has.Count.EqualTo(1));
            Assert.That(definitions[0].ToLowerInvariant(), Does.Contain("newsequentialid"));
            Assert.That(definitions[0].ToLowerInvariant(), Does.Not.Contain("newid("));
        });
    }

    [Test]
    public async Task AuditTimestamps_OnInsert_PopulatedBySysUtcDateTimeDefault()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser user = PersistenceTestData.AddUser(context, "ts-creator");
        await context.SaveChangesAsync(CancellationToken.None);
        string insertSql =
            "INSERT INTO ForumThreads (Title, CreatedById) " +
            $"OUTPUT INSERTED.CreatedAtUtc VALUES ('default-ts-thread', '{user.Id}')";

        // Act
        DateTime serverGeneratedTimestamp = await ScalarDateTimeAsync(context, insertSql);

        // Assert
        Assert.That(
            serverGeneratedTimestamp,
            Is.GreaterThan(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)));
    }

    [Test]
    public async Task Schema_TablesKeysAndIndexes_UseExplicitPkFkIxNaming()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();

        // Act
        IReadOnlyList<string> primaryKeys = await QueryNamesAsync(context,
            "SELECT name FROM sys.key_constraints WHERE type = 'PK'");
        IReadOnlyList<string> foreignKeys = await QueryNamesAsync(context,
            "SELECT name FROM sys.foreign_keys");
        IReadOnlyList<string> indexes = await QueryNamesAsync(context,
            "SELECT name FROM sys.indexes WHERE name IS NOT NULL");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(primaryKeys, Does.Contain("PK_Comments"));
            Assert.That(primaryKeys, Does.Contain("PK_ForumThreads"));
            Assert.That(primaryKeys, Does.Contain("PK_ModerationDecisions"));
            Assert.That(foreignKeys, Does.Contain("FK_Comments_ForumThreads"));
            Assert.That(foreignKeys, Does.Contain("FK_Comments_AspNetUsers"));
            Assert.That(foreignKeys, Does.Contain("FK_ModerationDecisions_Comments"));
            Assert.That(indexes, Does.Contain("IX_Comments_Status"));
            Assert.That(indexes, Does.Contain("IX_Comments_ThreadId"));
            Assert.That(indexes, Does.Contain("IX_ModerationDecisions_CommentId"));
        });
    }

    [Test]
    public async Task Comment_BodyExceedingMaxLength_RejectedByDatabase()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(context);
        string overlongBody = new('x', Comment.MaxBodyLength + 50);
        Comment comment = (Comment)Activator.CreateInstance(typeof(Comment), nonPublic: true)!;
        SetCommentRawColumns(context, comment, threadId, authorId, overlongBody);

        // Act & Assert
        Assert.That(
            async () => await context.SaveChangesAsync(CancellationToken.None),
            Throws.InstanceOf<DbUpdateException>());
    }

    [Test]
    public async Task ForumThread_TitleExceedingMaxLength_RejectedByDatabase()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser user = PersistenceTestData.AddUser(context, "title-creator");
        await context.SaveChangesAsync(CancellationToken.None);

        ForumThread thread = (ForumThread)Activator.CreateInstance(typeof(ForumThread), nonPublic: true)!;
        SetThreadRawColumns(context, thread, new string('t', ForumThread.MaxTitleLength + 25), user.Id);

        // Act & Assert
        Assert.That(
            async () => await context.SaveChangesAsync(CancellationToken.None),
            Throws.InstanceOf<DbUpdateException>());
    }

    [Test]
    public async Task EnumColumns_StatusLabelDecision_PersistAsIntAndRoundTrip()
    {
        // Arrange
        await using ForumGuardDbContext writeContext = CreateContext();
        (Guid threadId, Guid moderatorId) = await SeedThreadAndAuthorAsync(writeContext);
        Comment comment = new(threadId, moderatorId, "enum body", BaseInstant);
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        writeContext.Comments.Add(comment);
        ModerationDecision decision = new(comment.Id, moderatorId, ModerationOutcome.Rejected, BaseInstant, "spam");
        writeContext.ModerationDecisions.Add(decision);
        await writeContext.SaveChangesAsync(CancellationToken.None);

        // Act
        await using ForumGuardDbContext readContext = CreateContext();
        int rawStatus = await ScalarIntAsync(readContext,
            $"SELECT CAST(Status AS int) FROM Comments WHERE Id = '{comment.Id}'");
        int rawDecision = await ScalarIntAsync(readContext,
            $"SELECT CAST(Decision AS int) FROM ModerationDecisions WHERE Id = '{decision.Id}'");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(rawStatus, Is.EqualTo((int)CommentStatus.FlaggedForReview));
            Assert.That(rawDecision, Is.EqualTo((int)ModerationOutcome.Rejected));
        });
    }

    [Test]
    public async Task UnitOfWork_SaveChangesAsync_PersistsTrackedChangesAndReturnsRowCount()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(context);
        CommentRepository repository = new(context);
        UnitOfWork unitOfWork = new(context);
        repository.Add(new Comment(threadId, authorId, "uow comment one", BaseInstant));
        repository.Add(new Comment(threadId, authorId, "uow comment two", BaseInstant));

        // Act
        int affected = await unitOfWork.SaveChangesAsync(CancellationToken.None);

        // Assert
        Assert.That(affected, Is.EqualTo(2));
    }

    [Test]
    public async Task Repository_ChangesNotPersisted_UntilUnitOfWorkSaveChangesAsync()
    {
        // Arrange
        await using ForumGuardDbContext writeContext = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(writeContext);
        Comment comment = new(threadId, authorId, "not yet saved", BaseInstant);
        new CommentRepository(writeContext).Add(comment);

        // Act
        await using ForumGuardDbContext probeContext = CreateContext();
        Comment? beforeSave = await new CommentRepository(probeContext).GetByIdAsync(comment.Id, CancellationToken.None);

        // Assert
        Assert.That(beforeSave, Is.Null);
    }

    [Test]
    public async Task DeleteForumThread_CascadesToCommentsAndModerationDecisions()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser user = PersistenceTestData.AddUser(context, "cascade-user");
        await context.SaveChangesAsync(CancellationToken.None);
        ForumThread thread = PersistenceTestData.AddThread(context, user.Id, "cascade thread");
        await context.SaveChangesAsync(CancellationToken.None);
        Comment comment = new(thread.Id, user.Id, "cascade comment", BaseInstant);
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        context.Comments.Add(comment);
        context.ModerationDecisions.Add(new ModerationDecision(comment.Id, user.Id, ModerationOutcome.Rejected, BaseInstant));
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        context.ForumThreads.Remove(thread);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        await using ForumGuardDbContext verifyContext = CreateContext();
        int remainingComments = await ScalarIntAsync(verifyContext,
            $"SELECT COUNT(*) FROM Comments WHERE ThreadId = '{thread.Id}'");
        int remainingDecisions = await ScalarIntAsync(verifyContext,
            $"SELECT COUNT(*) FROM ModerationDecisions WHERE CommentId = '{comment.Id}'");
        Assert.Multiple(() =>
        {
            Assert.That(remainingComments, Is.EqualTo(0));
            Assert.That(remainingDecisions, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task DeleteApplicationUser_WithAuthoredComments_RestrictedByForeignKey()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser user = PersistenceTestData.AddUser(context, "restrict-user");
        await context.SaveChangesAsync(CancellationToken.None);
        ForumThread thread = PersistenceTestData.AddThread(context, user.Id, "restrict thread");
        await context.SaveChangesAsync(CancellationToken.None);
        context.Comments.Add(new Comment(thread.Id, user.Id, "authored comment", BaseInstant));
        await context.SaveChangesAsync(CancellationToken.None);

        Guid userId = user.Id;

        // Act & Assert
        Assert.That(
            async () => await context.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM AspNetUsers WHERE Id = {userId}", CancellationToken.None),
            Throws.InstanceOf<DbException>());
    }

    [Test]
    public async Task InsertComment_WithNonexistentThreadId_ThrowsDbUpdateException()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        ApplicationUser author = PersistenceTestData.AddUser(context, "orphan-author");
        await context.SaveChangesAsync(CancellationToken.None);
        Comment comment = new(Guid.NewGuid(), author.Id, "orphan comment", BaseInstant);
        context.Comments.Add(comment);

        // Act & Assert
        Assert.That(
            async () => await context.SaveChangesAsync(CancellationToken.None),
            Throws.InstanceOf<DbUpdateException>());
    }

    [Test]
    public async Task ConcurrentModeration_SecondSaveChanges_ThrowsDbUpdateConcurrencyException()
    {
        // Arrange
        await using ForumGuardDbContext seedContext = CreateContext();
        (Guid threadId, Guid moderatorId) = await SeedThreadAndAuthorAsync(seedContext);
        Comment seedComment = new(threadId, moderatorId, "contested comment", BaseInstant);
        seedComment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.95f), BaseInstant);
        seedContext.Comments.Add(seedComment);
        await seedContext.SaveChangesAsync(CancellationToken.None);
        Guid commentId = seedComment.Id;

        await using ForumGuardDbContext firstContext = CreateContext();
        await using ForumGuardDbContext secondContext = CreateContext();
        Comment first = (await firstContext.Comments.FindAsync([commentId], CancellationToken.None))!;
        Comment second = (await secondContext.Comments.FindAsync([commentId], CancellationToken.None))!;

        first.Approve(BaseInstant.AddMinutes(5));
        await firstContext.SaveChangesAsync(CancellationToken.None);
        second.Reject();

        // Act & Assert
        Assert.That(
            async () => await secondContext.SaveChangesAsync(CancellationToken.None),
            Throws.InstanceOf<DbUpdateConcurrencyException>());
    }

    [Test]
    public async Task Repository_GetByIdAsync_MissingRow_ReturnsNull()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        CommentRepository repository = new(context);

        // Act
        Comment? result = await repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Repository_CountAsync_EmptyResult_ReturnsZero()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        CommentRepository repository = new(context);

        // Act
        int count = await repository.CountAsync(
            new PublishedCommentsByThreadSpecification(Guid.NewGuid()), CancellationToken.None);

        // Assert
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public async Task Repository_ListAsync_CancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        CommentRepository repository = new(context);
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        // Act & Assert
        Assert.That(
            async () => await repository.ListAsync(new FlaggedPendingReviewSpecification(), cancellationTokenSource.Token),
            Throws.InstanceOf<OperationCanceledException>());
    }

    [Test]
    public async Task FlaggedPendingReviewSpecification_AgainstDatabase_ReturnsOnlyQueueOrderedOldestFirst()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(context);
        Comment olderFlagged = new(threadId, authorId, "older flagged", BaseInstant.AddMinutes(1));
        olderFlagged.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        Comment newerFlagged = new(threadId, authorId, "newer flagged", BaseInstant.AddMinutes(10));
        newerFlagged.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        Comment publishedClean = new(threadId, authorId, "clean published", BaseInstant.AddMinutes(5));
        publishedClean.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, 0.1f), BaseInstant);
        context.Comments.AddRange(olderFlagged, newerFlagged, publishedClean);
        await context.SaveChangesAsync(CancellationToken.None);
        CommentRepository repository = new(context);

        // Act
        IReadOnlyList<Comment> queue = await repository.GetFlaggedQueueAsync(CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(queue, Has.Count.EqualTo(2));
            Assert.That(queue, Has.All.Property(nameof(Comment.Status)).EqualTo(CommentStatus.FlaggedForReview));
            Assert.That(queue[0].CreatedAtUtc, Is.LessThan(queue[1].CreatedAtUtc));
        });
    }

    [Test]
    public async Task PublishedCommentsByThreadSpecification_AgainstDatabase_ReturnsVisibleCommentsOnly()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        (Guid threadId, Guid authorId) = await SeedThreadAndAuthorAsync(context);
        Comment published = new(threadId, authorId, "published comment", BaseInstant.AddMinutes(1));
        published.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Clean, 0.1f), BaseInstant.AddMinutes(1));
        Comment approved = new(threadId, authorId, "approved comment", BaseInstant.AddMinutes(2));
        approved.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        approved.Approve(BaseInstant.AddMinutes(3));
        Comment flagged = new(threadId, authorId, "flagged comment", BaseInstant.AddMinutes(4));
        flagged.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        context.Comments.AddRange(published, approved, flagged);
        await context.SaveChangesAsync(CancellationToken.None);
        CommentRepository repository = new(context);

        // Act
        IReadOnlyList<Comment> visible = await repository.GetPublishedByThreadAsync(threadId, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(visible, Has.Count.EqualTo(2));
            Assert.That(visible.All(comment => comment.IsPubliclyVisible), Is.True);
            Assert.That(visible.Any(comment => comment.Status == CommentStatus.FlaggedForReview), Is.False);
        });
    }

    [Test]
    public async Task ActiveUsersSpecification_AgainstDatabase_ReturnsOnlyActiveUsers()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        PersistenceTestData.AddUser(context, "active-one", isActive: true);
        PersistenceTestData.AddUser(context, "active-two", isActive: true);
        PersistenceTestData.AddUser(context, "inactive-one", isActive: false);
        await context.SaveChangesAsync(CancellationToken.None);
        Repository<ApplicationUser> repository = new(context);

        // Act
        IReadOnlyList<ApplicationUser> active = await repository.ListAsync(
            new ActiveUsersSpecification(), CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(active, Has.Count.EqualTo(2));
            Assert.That(active, Has.All.Property(nameof(ApplicationUser.IsActive)).True);
        });
    }

    [Test]
    public async Task ModerationDecisionRepository_GetByCommentIdAsync_ReturnsDecisionsForComment()
    {
        // Arrange
        await using ForumGuardDbContext context = CreateContext();
        (Guid threadId, Guid moderatorId) = await SeedThreadAndAuthorAsync(context);
        Comment comment = new(threadId, moderatorId, "decided comment", BaseInstant);
        comment.ApplyAnalysis(new AnalysisResult(ToxicityLabel.Toxic, 0.9f), BaseInstant);
        context.Comments.Add(comment);
        context.ModerationDecisions.Add(new ModerationDecision(comment.Id, moderatorId, ModerationOutcome.Approved, BaseInstant, "looks fine"));
        await context.SaveChangesAsync(CancellationToken.None);
        ModerationDecisionRepository repository = new(context);

        // Act
        IReadOnlyList<ModerationDecision> decisions = await repository.GetByCommentIdAsync(comment.Id, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(decisions, Has.Count.EqualTo(1));
            Assert.That(decisions[0].Decision, Is.EqualTo(ModerationOutcome.Approved));
            Assert.That(decisions[0].Reason, Is.EqualTo("looks fine"));
        });
    }

    private async Task<(Guid ThreadId, Guid AuthorId)> SeedThreadAndAuthorAsync(ForumGuardDbContext context)
    {
        ApplicationUser author = PersistenceTestData.AddUser(context, "author");
        await context.SaveChangesAsync(CancellationToken.None);
        ForumThread thread = PersistenceTestData.AddThread(context, author.Id);
        await context.SaveChangesAsync(CancellationToken.None);
        return (thread.Id, author.Id);
    }

    private static void SetCommentRawColumns(ForumGuardDbContext context, Comment comment, Guid threadId, Guid authorId, string body)
    {
        EntityEntry entry = context.Comments.Add(comment);
        entry.Property(nameof(Comment.ThreadId)).CurrentValue = threadId;
        entry.Property(nameof(Comment.AuthorId)).CurrentValue = authorId;
        entry.Property(nameof(Comment.Body)).CurrentValue = body;
        entry.Property(nameof(Comment.Status)).CurrentValue = CommentStatus.PendingAnalysis;
        entry.Property(nameof(Comment.CreatedAtUtc)).CurrentValue = BaseInstant;
    }

    private static void SetThreadRawColumns(ForumGuardDbContext context, ForumThread thread, string title, Guid createdById)
    {
        EntityEntry entry = context.ForumThreads.Add(thread);
        entry.Property(nameof(ForumThread.Title)).CurrentValue = title;
        entry.Property(nameof(ForumThread.CreatedById)).CurrentValue = createdById;
        entry.Property(nameof(ForumThread.CreatedAtUtc)).CurrentValue = BaseInstant;
    }

    private static async Task<int> ScalarIntAsync(ForumGuardDbContext context, string sql)
    {
        return Convert.ToInt32(await ScalarAsync(context, sql));
    }

    private static async Task<Guid> ScalarGuidAsync(ForumGuardDbContext context, string sql)
    {
        return (Guid)(await ScalarAsync(context, sql))!;
    }

    private static async Task<DateTime> ScalarDateTimeAsync(ForumGuardDbContext context, string sql)
    {
        return (DateTime)(await ScalarAsync(context, sql))!;
    }

    private static async Task<object?> ScalarAsync(ForumGuardDbContext context, string sql)
    {
        await using System.Data.Common.DbCommand command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        await context.Database.OpenConnectionAsync(CancellationToken.None);
        try
        {
            return await command.ExecuteScalarAsync(CancellationToken.None);
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }

    private static async Task<IReadOnlyList<string>> QueryNamesAsync(ForumGuardDbContext context, string sql)
    {
        List<string> names = [];
        await using System.Data.Common.DbCommand command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;
        await context.Database.OpenConnectionAsync(CancellationToken.None);
        try
        {
            await using System.Data.Common.DbDataReader reader = await command.ExecuteReaderAsync(CancellationToken.None);
            while (await reader.ReadAsync(CancellationToken.None))
            {
                names.Add(reader.GetString(0));
            }
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }

        return names;
    }
}
