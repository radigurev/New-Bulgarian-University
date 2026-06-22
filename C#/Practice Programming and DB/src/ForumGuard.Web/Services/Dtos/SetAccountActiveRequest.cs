namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Carries an Administrator request to activate or deactivate a target account (SDD-FORUM-003 B9..B17).
/// <para>The self-deactivation guard (B16) compares <see cref="TargetUserId"/> with
/// <see cref="ActingAdministratorId"/> when <see cref="Activate"/> is <c>false</c>.</para>
/// </summary>
/// <param name="TargetUserId">The identifier of the account whose state is being changed.</param>
/// <param name="ActingAdministratorId">The identifier of the Administrator performing the change.</param>
/// <param name="Activate">Whether to activate (<c>true</c>) or deactivate (<c>false</c>) the target.</param>
public sealed record SetAccountActiveRequest(Guid TargetUserId, Guid ActingAdministratorId, bool Activate);
