using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.Login;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<Result<AuthSessionData>>;