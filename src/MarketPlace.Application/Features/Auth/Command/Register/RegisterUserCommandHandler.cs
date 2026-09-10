using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.Register;


public class RegisterUserHandler(IAuthService authService) : IRequestHandler<RegisterUserCommand, Result<AuthSessionData>>
{
    public async Task<Result<AuthSessionData>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
      var data = new RegisterUserCommandData(
        request.FirstName,
        request.LastName,
        request.Email,
        request.Password
      );
        var result = await authService.RegisterUser(data, cancellationToken);
      if(result.IsSuccess)
      {
        return Result<AuthSessionData>.Success(result.Value);
      }
      else
      {
        return Result<AuthSessionData>.Failure(UserError.UserCreationFailed());
      }
    }
}