using MarketPlace.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Infrastructure.Identity.Services;

public class HttpContextProvider : IHttpContextProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentIpAddress()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return null;

        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            return forwardedHeader.Split(',').FirstOrDefault()?.Trim();
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        return ipAddress == "::1" ? "127.0.0.1" : ipAddress;
    }
}
