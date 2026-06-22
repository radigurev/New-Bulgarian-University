using System.ComponentModel.DataAnnotations;

namespace ForumGuard.Application.Options;

/// <summary>
/// Represents the strongly typed moderation configuration bound from the <c>"Moderation"</c> section.
/// <para>Constraints are enforced at startup; see <see cref="ModerationOptionsValidator"/>.</para>
/// </summary>
public sealed class ModerationOptions
{
    /// <summary>
    /// The inclusive upper bound the domain places on <see cref="MaxCommentLength"/>.
    /// </summary>
    public const int DomainMaxCommentLength = 4000;

    /// <summary>
    /// Gets or sets the inclusive toxic-score cutoff in the range <c>[0.0, 1.0]</c>; defaults to <c>0.5</c>.
    /// </summary>
    [Range(0.0, 1.0, ErrorMessage = "Moderation:ToxicityThreshold must be within [0.0, 1.0].")]
    public float ToxicityThreshold { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the path to the trained model file; required and non-whitespace.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Moderation:ModelPath must be a non-empty path.")]
    public string ModelPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum permitted comment length; positive and at most the domain cap.
    /// </summary>
    [Range(1, DomainMaxCommentLength, ErrorMessage = "Moderation:MaxCommentLength must be a positive integer not exceeding the domain cap of 4000.")]
    public int MaxCommentLength { get; set; } = DomainMaxCommentLength;

    /// <summary>
    /// Gets or sets the path to the profanity keyword list file; required and non-whitespace.
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Moderation:ProfanityListPath must be a non-empty path.")]
    public string ProfanityListPath { get; set; } = string.Empty;
}
