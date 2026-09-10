using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Infrastructure.Identity.Models;
using MarketPlace.Infrastructure.Persistence;
using MarketPlace.Infrastructure.Persistence.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;


namespace MarketPlace.Infrastructure.Identity.Services;


public class IdentityAuthService(
IOptions<JwtOptions> _jwtOptions,
UserManager<ApplicationUser> _userManager,
RoleManager<IdentityRole<Guid>> _roleManager,
JwtTokenService _jwtService,
ApplicationDbContext _dbContext,
IHttpContextProvider httpContextProvider

) : IAuthService
{
    public async Task<Result<AuthSessionData>> RegisterUser(RegisterUserCommandData request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {

            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName

        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return Result.Failure<AuthSessionData>(UserError.UserCreationFailed());
        }
        var token = await CreateSessionAsync(user, cancellationToken);
        await AddToRoleAsync(user.Id, UserRole.Customer);

        return new AuthSessionData(

             user.Id,
             user.Email!,
             token.AccessToken,
             token.AccessTokenExpiresAtUtc,
             token.RefreshToken,
             token.RefreshTokenExpiresAtUtc
         );

    }

    public async Task<Result<AuthSessionData>> LoginAsync(LoginUserCommandData request, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return Result.Failure<AuthSessionData>(UserError.InvalidUserCredentials());
        }

        var token = await CreateSessionAsync(user, cancellationToken);

        return new AuthSessionData(
            user.Id,
            user.Email!,
            token.AccessToken,
            token.AccessTokenExpiresAtUtc,
            token.RefreshToken,
            token.RefreshTokenExpiresAtUtc
        );



    }

    public async Task<Result<AuthSessionData>> RefreshTokenAsync(RefreshTokenCommandData request, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtService.HashToken(request.RefreshToken);
        var refreshToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(token => token.Token == tokenHash, cancellationToken);

        if (refreshToken == null)
        {
            return Result.Failure<AuthSessionData>(UserError.InvalidRefreshToken());

        }

        if (!refreshToken.IsActive)
        {
            return Result.Failure<AuthSessionData>(UserError.RefreshTokenRevokedOrExpired());
        }

        var user = await _userManager.FindByIdAsync(refreshToken.ApplicationUserId.ToString());
        if (user == null)
        {
            return Result.Failure<AuthSessionData>(UserError.UserNotFound());
        }

        var roles = await _userManager.GetRolesAsync(user);
        var newRefreshToken = _jwtService.CreateRefreshToken();
        var newRefreshTokenHash = _jwtService.HashToken(newRefreshToken);
        var expiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenDays);

        refreshToken.Rotate(refreshToken, newRefreshTokenHash, expiresAtUtc, request.IpAddress);
        _dbContext.RefreshTokens.Add(RefreshToken.Create(user.Id, newRefreshTokenHash, request.IpAddress, expiresAtUtc));
        await _dbContext.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtService.CreateAccessToken(user, roles, DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.Value.AccessTokenMinutes));

        return new AuthSessionData(
            user.Id,
            user.Email!,
            accessToken,
            DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.Value.AccessTokenMinutes),
            newRefreshToken,
            expiresAtUtc);
    }

    private async Task<AuthSessionData> CreateSessionAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var refreshToken = _jwtService.CreateRefreshToken();
        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.Value.RefreshTokenDays);
        var refreshTokenHash = _jwtService.HashToken(refreshToken);
        var ipAddress = httpContextProvider.GetCurrentIpAddress() ?? "Unknown";
        _dbContext.RefreshTokens.Add(RefreshToken.Create(user.Id, refreshTokenHash, ipAddress, refreshTokenExpiresAtUtc));
        await _dbContext.SaveChangesAsync(cancellationToken);
        return _jwtService.CreateSession(user, roles, refreshToken);
    }

    public Task<Result> LogoutAsync(LogoutCommandData request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ForgotPasswordAsync(ForgotPasswordCommandData request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ResetPasswordAsync(ResetPasswordCommandData request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }


    Task<bool> IAuthService.IsInRoleAsync(string userId, string roleName)
    {
        var user = _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        var isInRole = _userManager.IsInRoleAsync(user.Result, roleName).Result;
        return Task.FromResult(isInRole);
    }

    public async Task<bool> AddToRoleAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;

    }

    Task<List<string>> IAuthService.GetUserRolesAsync(string userId, CancellationToken cancellationToken)
    {
        var user = _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }
        var userRole = _userManager.AccessFailedAsync(user.Result).Result;
        var roles = _userManager.GetRolesAsync(user.Result).Result;
        return Task.FromResult(roles.ToList());
    }
}
