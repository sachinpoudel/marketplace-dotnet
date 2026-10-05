using MarketPlace.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Infrastructure.Identity.Services;

public class CurrentUserService( IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value ?? httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name;


    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;


public Guid GetCurrentUserId()
{
    if (IsAuthenticated == false || httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == false)
    {
        throw new InvalidOperationException("User is not authenticated.");
    }

    var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
    {
        throw new InvalidOperationException("User ID claim is missing or invalid.");
    }

    return userId;
}
}