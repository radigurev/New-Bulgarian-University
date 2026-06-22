using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class GameNightConfiguration : IEntityTypeConfiguration<GameNight>
{
    public void Configure(EntityTypeBuilder<GameNight> builder)
    {
        builder.ToTable("GameNights");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(n => n.Title).HasMaxLength(100).IsRequired();
        builder.Property(n => n.Location).HasMaxLength(200).IsRequired();
        builder.Property(n => n.Notes).HasMaxLength(1000);
        builder.Property(n => n.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.ScheduledForUtc).HasColumnType("datetime2(7)");
        builder.Property(n => n.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(n => n.UpdatedAtUtc).HasColumnType("datetime2(7)");
        builder.Property(n => n.CancelledAtUtc).HasColumnType("datetime2(7)");

        builder.HasIndex(n => n.HostUserId).HasDatabaseName("IX_GameNights_HostUserId");
        builder.HasIndex(n => n.ScheduledForUtc).HasDatabaseName("IX_GameNights_ScheduledForUtc");
        builder.HasIndex(n => n.Status).HasDatabaseName("IX_GameNights_Status");

        builder.HasOne(n => n.Host)
            .WithMany(u => u.HostedNights)
            .HasForeignKey(n => n.HostUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_GameNights_ExpectedPlayers", "[ExpectedPlayerCount] BETWEEN 2 AND 20"));
    }
}
