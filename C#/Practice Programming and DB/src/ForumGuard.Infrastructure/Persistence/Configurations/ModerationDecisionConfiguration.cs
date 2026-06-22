using ForumGuard.Domain.Entities;
using ForumGuard.Domain.Enums;
using ForumGuard.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumGuard.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the <see cref="ModerationDecision"/> entity mapping for EF Core.
/// <para>See SDD-FORUM-022. Configures explicit names, the <c>NEWSEQUENTIALID()</c> GUID key,
/// the <c>int</c>-stored outcome enum, a cascade foreign key to the decided comment, a restrict
/// foreign key to the moderator, and the lookup index by comment.</para>
/// </summary>
public sealed class ModerationDecisionConfiguration : IEntityTypeConfiguration<ModerationDecision>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ModerationDecision> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ConfigureTableAndColumns(builder);
        ConfigureRelationships(builder);

        builder.HasIndex(decision => decision.CommentId)
            .HasDatabaseName("IX_ModerationDecisions_CommentId");

        builder.HasIndex(decision => decision.ModeratorId)
            .HasDatabaseName("IX_ModerationDecisions_ModeratorId");
    }

    private static void ConfigureTableAndColumns(EntityTypeBuilder<ModerationDecision> builder)
    {
        builder.ToTable("ModerationDecisions");
        builder.HasKey(decision => decision.Id).HasName("PK_ModerationDecisions");

        builder.Property(decision => decision.Id)
            .HasColumnName("Id")
            .HasColumnType("uniqueidentifier")
            .HasDefaultValueSql("NEWSEQUENTIALID()")
            .ValueGeneratedOnAdd();

        builder.Property(decision => decision.CommentId)
            .HasColumnName("CommentId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(decision => decision.ModeratorId)
            .HasColumnName("ModeratorId")
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(decision => decision.Decision)
            .HasColumnName("Decision")
            .HasColumnType("int")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(decision => decision.DecidedAtUtc)
            .HasColumnName("DecidedAtUtc")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.Property(decision => decision.Reason)
            .HasColumnName("Reason")
            .HasColumnType("nvarchar(500)")
            .HasMaxLength(ModerationDecision.MaxReasonLength);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<ModerationDecision> builder)
    {
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(decision => decision.CommentId)
            .HasConstraintName("FK_ModerationDecisions_Comments")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(decision => decision.ModeratorId)
            .HasConstraintName("FK_ModerationDecisions_AspNetUsers")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
