namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Carries the server-trusted input required to self-register an account (SDD-FORUM-003 B1..B5).
/// <para>Only the fields here are accepted; <c>IsActive</c>, role, <c>CreatedAtUtc</c>, and <c>Id</c>
/// are server-controlled and never bound from the client.</para>
/// </summary>
/// <param name="Email">The account email, used as the username; required and unique.</param>
/// <param name="DisplayName">The display name shown across the forum; required, at most 128 characters.</param>
/// <param name="Password">The plaintext password; validated against the Identity policy, never persisted.</param>
public sealed record RegisterAccountRequest(string Email, string DisplayName, string Password);
