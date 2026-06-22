namespace ForumGuard.Domain.Enums;

/// <summary>
/// Represents the binary toxicity classification produced for a comment.
/// </summary>
public enum ToxicityLabel
{
    /// <summary>
    /// The analyzed text was classified as non-toxic.
    /// </summary>
    Clean,

    /// <summary>
    /// The analyzed text was classified as toxic.
    /// </summary>
    Toxic
}
