using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.Login;

public  class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthSessionData>>
{
    private readonly IAuthService _authService;

    public LoginUserHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<Result<AuthSessionData>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {

        var data = new LoginUserCommandData(
            request.Email, request.Password
        );
        var result = await _authService.LoginAsync(data, cancellationToken);

        if (!result.IsSuccess)
        {
            return Result<AuthSessionData>.Failure(UserError.InvalidUserCredentials());
        }

        return Result<AuthSessionData>.Success(result.Value);
    }
}