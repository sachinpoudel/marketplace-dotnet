using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.ChangeQuantity;

public record class ChangeQuantityCommand (
    Guid CartId,
    Guid ProductId,
    int Quantity
) : IRequest<Result<CartDetailDto>>;
