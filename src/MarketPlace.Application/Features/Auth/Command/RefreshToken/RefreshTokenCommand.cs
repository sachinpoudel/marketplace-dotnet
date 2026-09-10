using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Auth.Command.RefreshToken;


public record RefreshTokenCommand(
    string RefreshToken
 
) : IRequest<Result<AuthSessionData>>;