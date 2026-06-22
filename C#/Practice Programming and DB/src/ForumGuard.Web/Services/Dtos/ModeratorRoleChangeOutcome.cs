namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Classifies the result of a moderator-role grant or revoke (SDD-FORUM-004 §2.1..§2.4).
/// </summary>
public enum ModeratorRoleChangeOutcome
{
    /// <summary>
    /// The Moderator role was added to a user who did not previously hold it.
    /// </summary>
    Granted,

    /// <summary>
    /// The Moderator role was removed from a user who held it.
    /// </summary>
    Revoked,

    /// <summary>
    /// The request was an idempotent no-op (already a Moderator on grant, or not a Moderator on revoke).
    /// </summary>
    NoOp
}
