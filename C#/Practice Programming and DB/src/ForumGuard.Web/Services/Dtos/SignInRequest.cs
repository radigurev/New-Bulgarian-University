namespace ForumGuard.Web.Services.Dtos;

/// <summary>
/// Carries the credentials supplied at sign-in (SDD-FORUM-003 B6, V3).
/// <para>Sign-in succeeds only when the account exists, the password matches, and the account is active.</para>
/// </summary>
/// <param name="Email">The account email used as the username.</param>
/// <param name="Password">The plaintext password to verify against the stored Identity hash.</param>
/// <param name="RememberMe">Whether to issue a persistent authentication cookie.</param>
public sealed record SignInRequest(string Email, string Password, bool RememberMe);
