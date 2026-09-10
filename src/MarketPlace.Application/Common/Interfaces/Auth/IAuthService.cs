using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Application.Common.Interfaces.Auth;


public interface IAuthService
{
    Task<Result<AuthSessionData>> RegisterUser(RegisterUserCommandData commandData, CancellationToken cancellationToken = default);

    Task<Result<AuthSessionData>> LoginAsync(LoginUserCommandData request, CancellationToken cancellationToken);

    Task<Result<AuthSessionData>> RefreshTokenAsync(RefreshTokenCommandData request, CancellationToken cancellationToken);

    Task<Result> LogoutAsync(LogoutCommandData request, CancellationToken cancellationToken);

    Task<Result> ForgotPasswordAsync(ForgotPasswordCommandData request, CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(ResetPasswordCommandData request, CancellationToken cancellationToken);
    
    //Roles
    
    Task<bool> IsInRoleAsync(string userId, string roleName);
      Task<bool> AddToRoleAsync(Guid userId, string roleName);
      Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
}