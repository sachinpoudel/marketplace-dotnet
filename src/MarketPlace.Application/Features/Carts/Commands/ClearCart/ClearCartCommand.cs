using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.ClearCart;

public record class ClearCartCommand (Guid CartId, Guid UserId): IRequest<Result>;

