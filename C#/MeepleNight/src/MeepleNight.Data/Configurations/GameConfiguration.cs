using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id).HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(g => g.Title).HasMaxLength(100).IsRequired();
        builder.Property(g => g.Description).HasMaxLength(2000);
        builder.Property(g => g.Category).HasConversion<string>().HasMaxLength(20);
        builder.Property(g => g.CoverImagePath).HasMaxLength(260);

        builder.Property(g => g.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(g => g.UpdatedAtUtc).HasColumnType("datetime2(7)");

        builder.HasIndex(g => g.Title).HasDatabaseName("IX_Games_Title");
        builder.HasIndex(g => new { g.IsActive, g.Category }).HasDatabaseName("IX_Games_IsActive_Category");

        // Filtered unique index — title must be unique among ACTIVE games (case-insensitive via SQL collation default)
        builder.HasIndex(g => g.Title)
            .IsUnique()
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("UX_Games_Title_Active");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Games_PlayerRange", "[MaxPlayers] >= [MinPlayers]");
            t.HasCheckConstraint("CK_Games_Duration", "[AverageDurationMinutes] BETWEEN 5 AND 720");
            t.HasCheckConstraint("CK_Games_PlayersBounds", "[MinPlayers] >= 1 AND [MaxPlayers] <= 20");
        });
    }
}
