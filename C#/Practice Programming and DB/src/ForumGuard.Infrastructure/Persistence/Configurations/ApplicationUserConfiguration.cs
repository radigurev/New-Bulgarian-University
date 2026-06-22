using ForumGuard.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForumGuard.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the ForumGuard extension columns on the ASP.NET Core Identity user table.
/// <para>See SDD-FORUM-022. The base Identity schema (table name, key, indexes) is owned by
/// <c>IdentityDbContext</c>; this configuration adds the <see cref="ApplicationUser.DisplayName"/>,
/// <see cref="ApplicationUser.IsActive"/> and <see cref="ApplicationUser.CreatedAtUtc"/> columns.</para>
/// </summary>
public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(user => user.DisplayName)
            .HasColumnName("DisplayName")
            .HasColumnType("nvarchar(256)")
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(user => user.CreatedAtUtc)
            .HasColumnName("CreatedAtUtc")
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.HasIndex(user => user.IsActive)
            .HasDatabaseName("IX_AspNetUsers_IsActive");
    }
}
