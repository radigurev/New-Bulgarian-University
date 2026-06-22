using Microsoft.Extensions.Options;

namespace ForumGuard.Application.Options;

/// <summary>
/// Validates <see cref="ModerationOptions"/> at startup, aggregating every field violation into a single result.
/// <para>Registered with <c>ValidateOnStart()</c> by the composition root so invalid configuration aborts host start.</para>
/// </summary>
public sealed class ModerationOptionsValidator : IValidateOptions<ModerationOptions>
{
    /// <summary>
    /// Validates the bound options, returning all field-specific failures together.
    /// </summary>
    /// <param name="name">The named-options name, or <c>null</c> for the default instance.</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A success result, or a failure listing every violated field and its constraint.</returns>
    public ValidateOptionsResult Validate(string? name, ModerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        List<string> failures = [];

        ValidateToxicityThreshold(options, failures);
        ValidateMaxCommentLength(options, failures);
        ValidatePath(options.ModelPath, ModerationConfigKeys.ModelPath, failures);
        ValidatePath(options.ProfanityListPath, ModerationConfigKeys.ProfanityListPath, failures);

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void ValidateToxicityThreshold(ModerationOptions options, List<string> failures)
    {
        if (float.IsNaN(options.ToxicityThreshold)
            || options.ToxicityThreshold < 0.0f
            || options.ToxicityThreshold > 1.0f)
        {
            failures.Add($"{ModerationConfigKeys.ToxicityThreshold} must be within [0.0, 1.0].");
        }
    }

    private static void ValidateMaxCommentLength(ModerationOptions options, List<string> failures)
    {
        if (options.MaxCommentLength < 1)
        {
            failures.Add($"{ModerationConfigKeys.MaxCommentLength} must be a positive integer.");
            return;
        }

        if (options.MaxCommentLength > ModerationOptions.DomainMaxCommentLength)
        {
            failures.Add($"{ModerationConfigKeys.MaxCommentLength} must not exceed the domain cap of {ModerationOptions.DomainMaxCommentLength}.");
        }
    }

    private static void ValidatePath(string value, string key, List<string> failures)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            failures.Add($"{key} must be a non-empty path.");
        }
    }
}
