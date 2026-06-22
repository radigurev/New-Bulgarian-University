using System.Security.Claims;

namespace ForumGuard.Tests.Application.Fakes;

/// <summary>
/// Builds <see cref="ClaimsPrincipal"/> instances carrying role claims for authorization and workspace tests.
/// <para>Produces authenticated principals with the supplied roles, or an anonymous principal.</para>
/// </summary>
public static class PrincipalFactory
{
    /// <summary>
    /// Creates an authenticated principal holding the supplied role claims.
    /// </summary>
    /// <param name="roles">The role names to assign.</param>
    /// <returns>An authenticated <see cref="ClaimsPrincipal"/>.</returns>
    public static ClaimsPrincipal WithRoles(params string[] roles)
    {
        List<Claim> claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
        ClaimsIdentity identity = new(claims, authenticationType: "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    /// <summary>
    /// Creates an anonymous, unauthenticated principal.
    /// </summary>
    /// <returns>An anonymous <see cref="ClaimsPrincipal"/>.</returns>
    public static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());
}
