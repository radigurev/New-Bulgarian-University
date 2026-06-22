using ForumGuard.Application.Moderation;
using ForumGuard.Domain.Interfaces;
using ForumGuard.Infrastructure.Analysis;
using ForumGuard.Infrastructure.Persistence;
using ForumGuard.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForumGuard.Infrastructure;

/// <summary>
/// Registers the ForumGuard Infrastructure-layer services with the dependency-injection container.
/// <para>Registers the EF Core <see cref="ForumGuardDbContext"/> over SQL Server, the repositories
/// and unit of work, and the keyword <see cref="ICommentAnalyzer"/> keyed under
/// <see cref="AnalyzerKeys.Keyword"/>. The full ASP.NET Core Identity registration (with
/// <c>SignInManager</c> and the application cookie) and the NAS-BERT analyzer are wired by the
/// composition root (Web).</para>
/// </summary>
public static class DependencyInjection
{
    private const string ConnectionStringName = "ForumGuardDb";

    /// <summary>
    /// Adds the Infrastructure-layer services to the supplied service collection.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration supplying the connection string.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        AddPersistence(services, configuration);
        AddRepositories(services);
        AddAnalyzers(services);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' was not found in configuration.");
        }

        services.AddDbContext<ForumGuardDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlServerOptions => sqlServerOptions.MigrationsAssembly(
                    typeof(ForumGuardDbContext).Assembly.FullName)));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IForumThreadRepository, ForumThreadRepository>();
        services.AddScoped<IModerationDecisionRepository, ModerationDecisionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddAnalyzers(IServiceCollection services)
    {
        services.AddKeyedSingleton<ICommentAnalyzer, KeywordCommentAnalyzer>(AnalyzerKeys.Keyword);
    }
}
