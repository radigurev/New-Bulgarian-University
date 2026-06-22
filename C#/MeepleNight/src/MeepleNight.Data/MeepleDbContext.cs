using MeepleNight.Domain.Common;
using MeepleNight.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeepleNight.Data;

public class MeepleDbContext
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public MeepleDbContext(DbContextOptions<MeepleDbContext> options) : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameNight> GameNights => Set<GameNight>();
    public DbSet<GameNightCandidate> GameNightCandidates => Set<GameNightCandidate>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SessionPlayer> SessionPlayers => Set<SessionPlayer>();
    public DbSet<Invitation> Invitations => Set<Invitation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Identity table renames are kept default; configurations only override fields we add.
    }

    public override int SaveChanges()
    {
        StampAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Sets CreatedAtUtc / UpdatedAtUtc on entities that implement IAuditable.</summary>
    private void StampAuditFields()
    {
        DateTime now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedAtUtc == default)
                {
                    entry.Entity.CreatedAtUtc = now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
            }
        }
    }
}
