using MeepleNight.Domain.Interfaces;
using System.Security.Claims;

namespace MeepleNight.Web.Middleware;

public sealed class CurrentUserAccessor : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            string? raw = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out Guid id) ? id : null;
        }
    }

    public string? DisplayName => Principal?.FindFirstValue("DisplayName")
                                  ?? Principal?.Identity?.Name;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public bool IsAdmin => Principal?.IsInRole("Admin") == true;
}
