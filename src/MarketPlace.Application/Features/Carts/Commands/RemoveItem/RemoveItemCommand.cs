using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.RemoveItem;

public record class RemoveItemCommand(Guid CartItemId) : IRequest<Result<CartDetailDto>>;
