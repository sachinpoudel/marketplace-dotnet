using System.Security.Claims;
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
    var user = httpContextAccessor.HttpContext?.User;

    if (user?.Identity?.IsAuthenticated != true)
    {
        throw new UnauthorizedAccessException("User is not authenticated.");
    }

    var userId =
        user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? user.FindFirstValue("sub");

    if (!Guid.TryParse(userId, out var id))
    {
        throw new UnauthorizedAccessException(
            "User ID claim is missing or invalid.");
    }

    return id;
}
}