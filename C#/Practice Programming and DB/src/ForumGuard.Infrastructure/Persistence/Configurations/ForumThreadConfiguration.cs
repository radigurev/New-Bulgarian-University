using ForumGuard.Domain.Entities;
using ForumGuard.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumGuard.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="ForumThread"/> entity mapping for EF Core.
/// <para>See SDD-FORUM-022. Explicit table/column names, <c>NEWSEQUENTIALID()</c> GUID key,
/// <c>DATETIME2(7)</c> audit timestamp and a restrict foreign key to the creating user.</para>
/// </summary>
public sealed class ForumThreadConfiguration : IEntityTypeConfiguration<ForumThread>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ForumThread> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("ForumThreads");
        builder.HasKey(thread => thread.Id).HasName("PK_ForumThreads");

        builder.Property(thread => thread.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(thread => thread.Title)
            .HasColumnName("Title")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(ForumThread.MaxTitleLength)
            .IsRequired();

        builder.Property(thread => thread.CreatedById)
            .HasColumnName("CreatedById")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(thread => thread.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(thread => thread.CreatedById)
            .HasConstraintName("FK_ForumThreads_AspNetUsers")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(thread => thread.CreatedById)
            .HasDatabaseName("IX_ForumThreads_CreatedById");
    }
}
