using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("Sessions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(s => s.StartedAtUtc).HasColumnType("datetime2(7)");
        builder.Property(s => s.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(s => s.UpdatedAtUtc).HasColumnType("datetime2(7)");
        builder.Property(s => s.WinnerNote).HasMaxLength(200);

        builder.HasOne(s => s.GameNight)
            .WithMany(n => n.Sessions)
            .HasForeignKey(s => s.GameNightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Game)
            .WithMany(g => g.Sessions)
            .HasForeignKey(s => s.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.GameNightId).HasDatabaseName("IX_Sessions_GameNightId");
        builder.HasIndex(s => s.GameId).HasDatabaseName("IX_Sessions_GameId");

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_Sessions_Duration", "[DurationMinutes] IS NULL OR ([DurationMinutes] BETWEEN 1 AND 720)"));
    }
}
