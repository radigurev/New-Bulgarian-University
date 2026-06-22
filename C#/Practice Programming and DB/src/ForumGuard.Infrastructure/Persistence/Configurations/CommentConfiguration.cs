using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumGuard.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="Comment"/> entity mapping for EF Core.
/// <para>See SDD-FORUM-022. Configures explicit names, the <c>NEWSEQUENTIALID()</c> GUID key,
/// <c>int</c>-stored enums, the <c>rowversion</c> concurrency token, cascade/restrict foreign keys
/// and the moderation-queue and thread-visibility indexes.</para>
/// </summary>
public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    /// <summary>
    /// The name of the shadow optimistic-concurrency token column.
    /// </summary>
    public const string RowVersionColumn = "RowVersion";

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ConfigureTableAndColumns(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureTableAndColumns(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");
        builder.HasKey(comment => comment.Id).HasName("PK_Comments");

        builder.Property(comment => comment.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(comment => comment.ThreadId)
            .HasColumnName("ThreadId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(comment => comment.AuthorId)
            .HasColumnName("AuthorId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(comment => comment.Body)
            .HasColumnName("Body")
            .HasColumnType("nvarchar(4000)")
            .HasMaxLength(Comment.MaxBodyLength)
            .IsRequired();

        builder.Property(comment => comment.Status)
            .HasColumnName("Status")
            .HasColumnType("int")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(comment => comment.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(comment => comment.PublishedAtUtc)
            .HasColumnName("PublishedAtUtc")
            .HasColumnType("datetime2(7)");

        builder.Property(comment => comment.AnalysisLabel)
            .HasColumnName("AnalysisLabel")
            .HasColumnType("int")
            .HasConversion<int?>();

        builder.Property(comment => comment.AnalysisScore)
            .HasColumnName("AnalysisScore")
            .HasColumnType("real");

        builder.Property(comment => comment.AnalyzedAtUtc)
            .HasColumnName("AnalyzedAtUtc")
            .HasColumnType("datetime2(7)");

        builder.Property<byte[]>(RowVersionColumn)
            .HasColumnName(RowVersionColumn)
            .HasColumnType("rowversion")
            .IsRowVersion();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Comment> builder)
    {
        builder.HasOne<ForumThread>()
            .WithMany()
            .HasForeignKey(comment => comment.ThreadId)
            .HasConstraintName("FK_Comments_ForumThreads")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(comment => comment.AuthorId)
            .HasConstraintName("FK_Comments_AspNetUsers")
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Comment> builder)
    {
        builder.HasIndex(comment => comment.Status)
            .HasDatabaseName("IX_Comments_Status");

        builder.HasIndex(comment => comment.ThreadId)
            .HasDatabaseName("IX_Comments_ThreadId");

        builder.HasIndex(comment => comment.AuthorId)
            .HasDatabaseName("IX_Comments_AuthorId");
    }
}
