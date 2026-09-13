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
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new InvalidOperationException("User is not authenticated or user ID claim is missing.");
        }

        return userId;
    }
}