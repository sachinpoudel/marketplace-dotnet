using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.RemoveItem;

public class RemoveItemCommandHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser
) : IRequestHandler<RemoveItemCommand, Result<CartDetailDto>>
{
    public async Task<Result<CartDetailDto>> Handle(
        RemoveItemCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        var cart = await cartRepository.GetCartByUserIdAsync(currentUserId, cancellationToken);
        if (cart == null)
        {
            return Result<CartDetailDto>.Failure(CartError.CartNotFound());
        }
        if (cart.Items.Count == 0)
        {
            return Result<CartDetailDto>.Failure(CartError.EmptyCart());
        }
        var itemToRemove = cart.Items.FirstOrDefault(i => i.Id.Equals(request.CartItemId));
        var itemId = itemToRemove?.Id;
        cart.RemoveItem(itemId);
        await cartRepository.UpdateAsync(cart, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<CartDetailDto>.Success(
            new CartDetailDto(
                cart.Id.Value,
                cart.UserId,
                cart.Items.Select(i => new CartItemDto(
                        i.ProductId.Value,
                        i.Quantity,
                        cart.Id.Value,
                        i.PriceAtTheTimeOfAdding
                    ))
                    .ToList(),
                itemToRemove?.Quantity ?? 0,
                cart.CalculateTotalPrice()
            )
        );
    }
}
