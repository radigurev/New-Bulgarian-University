using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("Invitations");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(i => i.RespondedAtUtc).HasColumnType("datetime2(7)");

        builder.HasIndex(i => new { i.GameNightId, i.InviteeUserId })
            .IsUnique()
            .HasDatabaseName("UX_Invitations_GameNight_Invitee");

        builder.HasIndex(i => i.InviteeUserId).HasDatabaseName("IX_Invitations_InviteeUserId");
        builder.HasIndex(i => i.GameNightId).HasDatabaseName("IX_Invitations_GameNightId");

        builder.HasOne(i => i.GameNight)
            .WithMany(n => n.Invitations)
            .HasForeignKey(i => i.GameNightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Invitee)
            .WithMany(u => u.Invitations)
            .HasForeignKey(i => i.InviteeUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.InvitedBy)
            .WithMany()
            .HasForeignKey(i => i.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
