using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.Register;


public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password
) : IRequest<Result<AuthSessionData>>;


