using MeepleNight.Domain.Interfaces;
using Serilog.Context;

namespace MeepleNight.Web.Middleware;

/// <summary>Pushes UserId / DisplayName into the Serilog log context for every request.</summary>
public sealed class CurrentUserLogContextMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserLogContextMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICurrentUserAccessor currentUser)
    {
        if (currentUser.IsAuthenticated && currentUser.UserId.HasValue)
        {
            using (LogContext.PushProperty("UserId", currentUser.UserId.Value))
            using (LogContext.PushProperty("UserDisplayName", currentUser.DisplayName ?? "(unknown)"))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
