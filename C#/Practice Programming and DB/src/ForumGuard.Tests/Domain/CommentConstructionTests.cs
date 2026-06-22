using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Tests.Builders;

namespace ForumGuard.Tests.Domain;

/// <summary>
/// Verifies <see cref="Comment"/> construction, initial state, and body validation per SDD-FORUM-010 §3.1, §4.1.
/// </summary>
[TestFixture]
[Category("SDD-FORUM-010")]
public sealed class CommentConstructionTests
{
    private static readonly DateTime CreatedAtUtc = new(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);

    [Test]
    public void NewComment_AfterConstruction_StatusIsPendingAnalysis()
    {
        // Arrange
        CommentBuilder builder = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc);

        // Act
        Comment comment = builder.Build();

        // Assert
        Assert.That(comment.Status, Is.EqualTo(CommentStatus.PendingAnalysis));
    }

    [Test]
    public void NewComment_AfterConstruction_AnalysisFieldsAndPublishedAtAreNull()
    {
        // Arrange
        CommentBuilder builder = CommentBuilder.Create().WithCreatedAtUtc(CreatedAtUtc);

        // Act
        Comment comment = builder.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.PublishedAtUtc, Is.Null);
            Assert.That(comment.AnalysisLabel, Is.Null);
            Assert.That(comment.AnalysisScore, Is.Null);
            Assert.That(comment.AnalyzedAtUtc, Is.Null);
        });
    }

    [Test]
    public void NewComment_AfterConstruction_RetainsProvidedFields()
    {
        // Arrange
        Guid threadId = Guid.NewGuid();
        Guid authorId = Guid.NewGuid();
        CommentBuilder builder = CommentBuilder.Create()
            .WithThreadId(threadId)
            .WithAuthorId(authorId)
            .WithBody("Hello forum.")
            .WithCreatedAtUtc(CreatedAtUtc);

        // Act
        Comment comment = builder.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(comment.ThreadId, Is.EqualTo(threadId));
            Assert.That(comment.AuthorId, Is.EqualTo(authorId));
            Assert.That(comment.Body, Is.EqualTo("Hello forum."));
            Assert.That(comment.CreatedAtUtc, Is.EqualTo(CreatedAtUtc));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("\t\n")]
    public void NewComment_BodyNullOrWhitespace_FailsValidation(string? body)
    {
        // Arrange
        Guid threadId = Guid.NewGuid();
        Guid authorId = Guid.NewGuid();

        // Act
        TestDelegate construction = () => new Comment(threadId, authorId, body!, CreatedAtUtc);

        // Assert
        Assert.That(construction, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void NewComment_BodyExceeds4000Chars_FailsValidation()
    {
        // Arrange
        string body = new('x', Comment.MaxBodyLength + 1);
        Guid threadId = Guid.NewGuid();
        Guid authorId = Guid.NewGuid();

        // Act
        TestDelegate construction = () => new Comment(threadId, authorId, body, CreatedAtUtc);

        // Assert
        Assert.That(construction, Throws.TypeOf<ArgumentException>());
    }

    [Test]
    public void NewComment_BodyAt4000Chars_PassesValidation()
    {
        // Arrange
        string body = new('x', Comment.MaxBodyLength);
        CommentBuilder builder = CommentBuilder.Create().WithBody(body).WithCreatedAtUtc(CreatedAtUtc);

        // Act
        Comment comment = builder.Build();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(comment.Body, Has.Length.EqualTo(Comment.MaxBodyLength));
            Assert.That(comment.Status, Is.EqualTo(CommentStatus.PendingAnalysis));
        });
    }

    [Test]
    public void NewComment_BodyAtOneChar_PassesValidation()
    {
        // Arrange
        CommentBuilder builder = CommentBuilder.Create().WithBody("x").WithCreatedAtUtc(CreatedAtUtc);

        // Act
        Comment comment = builder.Build();

        // Assert
        Assert.That(comment.Body, Has.Length.EqualTo(1));
    }
}
