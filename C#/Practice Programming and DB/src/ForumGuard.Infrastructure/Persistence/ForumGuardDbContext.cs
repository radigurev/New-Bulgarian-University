using ForumGuard.Domain.Entities;
using ForumGuard.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForumGuard.Infrastructure.Persistence;

/// <summary>
/// The Entity Framework Core database context for ForumGuard, combining the ASP.NET Core Identity
/// schema with the forum moderation entities.
/// <para>See SDD-FORUM-022. Maps <see cref="ForumThread"/>, <see cref="Comment"/> and
/// <see cref="ModerationDecision"/> alongside the Identity tables for <see cref="ApplicationUser"/>
/// and <see cref="IdentityRole{TKey}"/>.</para>
/// </summary>
public sealed class ForumGuardDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumGuardDbContext"/> class.
    /// </summary>
    /// <param name="options">The configured context options.</param>
    public ForumGuardDbContext(DbContextOptions<ForumGuardDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the set of forum threads.
    /// </summary>
    public DbSet<ForumThread> ForumThreads => Set<ForumThread>();

    /// <summary>
    /// Gets the set of comments.
    /// </summary>
    public DbSet<Comment> Comments => Set<Comment>();

    /// <summary>
    /// Gets the set of moderation decisions.
    /// </summary>
    public DbSet<ModerationDecision> ModerationDecisions => Set<ModerationDecision>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ForumGuardDbContext).Assembly);
    }
}
