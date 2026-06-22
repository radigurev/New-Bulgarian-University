using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class GameNightCandidateConfiguration : IEntityTypeConfiguration<GameNightCandidate>
{
    public void Configure(EntityTypeBuilder<GameNightCandidate> builder)
    {
        builder.ToTable("GameNightCandidates");
        builder.HasKey(c => new { c.GameNightId, c.GameId });

        builder.Property(c => c.Position).IsRequired();

        builder.HasOne(c => c.GameNight)
            .WithMany(n => n.Candidates)
            .HasForeignKey(c => c.GameNightId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Game)
            .WithMany(g => g.Candidates)
            .HasForeignKey(c => c.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
            t.HasCheckConstraint("CK_GameNightCandidates_Position", "[Position] BETWEEN 1 AND 10"));
    }
}
