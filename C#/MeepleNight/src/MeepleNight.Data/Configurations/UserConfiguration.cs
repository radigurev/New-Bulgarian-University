using MeepleNight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MeepleNight.Data.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.DisplayName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.AvatarPath)
            .HasMaxLength(260);

        builder.Property(u => u.CreatedAtUtc)
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(u => u.LastLoginAtUtc)
            .HasColumnType("datetime2(7)");

        builder.Property(u => u.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");
    }
}
