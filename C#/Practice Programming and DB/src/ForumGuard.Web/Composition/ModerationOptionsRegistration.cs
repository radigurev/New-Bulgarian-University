using ForumGuard.Application.Options;

namespace ForumGuard.Web.Composition;

/// <summary>
/// Binds and fail-fast-validates <see cref="ModerationOptions"/> at startup (SDD-FORUM-023).
/// <para>Binds the <c>"Moderation"</c> section, attaches DataAnnotations validation and the registered
/// <c>IValidateOptions</c> validator (from the Application layer), and calls <c>ValidateOnStart</c> so
/// an invalid configuration aborts host start.</para>
/// </summary>
public static class ModerationOptionsRegistration
{
    /// <summary>
    /// Registers the moderation options binding and fail-fast validation.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration supplying the section.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddModerationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<ModerationOptions>()
            .Bind(configuration.GetSection(ModerationConfigKeys.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
