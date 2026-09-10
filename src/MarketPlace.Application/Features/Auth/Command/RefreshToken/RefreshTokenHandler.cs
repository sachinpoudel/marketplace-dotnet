using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.RefreshToken;


public class RefreshTokenHandler(
IAuthService _authService,
IHttpContextProvider _httpContextProvider


) : IRequestHandler<RefreshTokenCommand, Result<AuthSessionData>>
{
    public async Task<Result<AuthSessionData>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var clientIpAddress = _httpContextProvider.GetCurrentIpAddress();
        var data = new RefreshTokenCommandData(request.RefreshToken, clientIpAddress);
        var result = await _authService.RefreshTokenAsync(data, cancellationToken);

        if(result.IsSuccess)
        {
            return Result<AuthSessionData>.Success(result.Value);
        }
        else
        {
            return Result<AuthSessionData>.Failure(UserError.InvalidRefreshToken());
        }
    }
}