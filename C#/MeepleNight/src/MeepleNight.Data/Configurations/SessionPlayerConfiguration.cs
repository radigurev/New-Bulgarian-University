using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class SessionPlayerConfiguration : IEntityTypeConfiguration<SessionPlayer>
{
    public void Configure(EntityTypeBuilder<SessionPlayer> builder)
    {
        builder.ToTable("SessionPlayers");
        builder.HasKey(p => new { p.SessionId, p.UserId });

        builder.Property(p => p.Score).HasColumnType("decimal(9,2)");
        builder.Property(p => p.Placement).IsRequired();

        builder.HasOne(p => p.Session)
            .WithMany(s => s.Players)
            .HasForeignKey(p => p.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.User)
            .WithMany(u => u.SessionParticipations)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.UserId).HasDatabaseName("IX_SessionPlayers_UserId");
        builder.HasIndex(p => new { p.UserId, p.Placement }).HasDatabaseName("IX_SessionPlayers_UserId_Placement");

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_SessionPlayers_Placement", "[Placement] >= 1"));
    }
}
