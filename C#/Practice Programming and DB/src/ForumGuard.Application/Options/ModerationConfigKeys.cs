namespace ForumGuard.Application.Options;

/// <summary>
/// Provides the authoritative configuration key constants for the moderation options.
/// <para>See <see cref="ModerationOptions"/> and <see cref="ModerationOptionsValidator"/>.</para>
/// </summary>
public static class ModerationConfigKeys
{
    /// <summary>
    /// The name of the configuration section that <see cref="ModerationOptions"/> binds from.
    /// </summary>
    public const string SectionName = "Moderation";

    /// <summary>
    /// The fully qualified key for the toxicity threshold value.
    /// </summary>
    public const string ToxicityThreshold = "Moderation:ToxicityThreshold";

    /// <summary>
    /// The fully qualified key for the model file path value.
    /// </summary>
    public const string ModelPath = "Moderation:ModelPath";

    /// <summary>
    /// The fully qualified key for the maximum comment length value.
    /// </summary>
    public const string MaxCommentLength = "Moderation:MaxCommentLength";

    /// <summary>
    /// The fully qualified key for the profanity list file path value.
    /// </summary>
    public const string ProfanityListPath = "Moderation:ProfanityListPath";

    /// <summary>
    /// The fully qualified connection-string key consumed by the data layer.
    /// </summary>
    public const string ForumGuardDbConnectionString = "ConnectionStrings:ForumGuardDb";
}
