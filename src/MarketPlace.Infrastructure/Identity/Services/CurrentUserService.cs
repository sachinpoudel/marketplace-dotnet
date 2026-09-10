using MarketPlace.Application.Common.Interfaces.Auth;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Infrastructure.Identity.Services;

public class CurrentUserService( IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;


    public string? UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name;
    

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}