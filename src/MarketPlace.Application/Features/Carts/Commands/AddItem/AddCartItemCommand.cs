using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.AddItem;

public record class AddCartItemCommand
(
   
    int Quantity,
    Guid ProductId
): IRequest<Result<CartDetailDto>>;
