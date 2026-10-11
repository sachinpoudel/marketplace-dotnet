using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.ChangeQuantity;

public class ChangeQuantityCommandHandler(
    IUnitOfWork unitOfWork,
    ICartRepository cartRepository,
    ICurrentUser currentUser,
    IProductRepository productRepository
) : IRequestHandler<ChangeQuantityCommand, Result<CartDetailDto>>
{
    public async Task<Result<CartDetailDto>> Handle(
        ChangeQuantityCommand request,
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
        if (cart.Items.All(i => i.ProductId.Value != ProductId.Create(request.ProductId).Value))
        {
            return Result<CartDetailDto>.Failure(CartError.FailedToAddItem());
        }
        var product = await productRepository.GetByIdAsync(
            ProductId.Create(request.ProductId),
            cancellationToken
        );

        var AvailableStock = product.StockQuantity;

        cart.ChangeQuantity(ProductId.Create(request.ProductId), request.Quantity, AvailableStock);
        await cartRepository.UpdateAsync(cart, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        var totalPrice = cart.CalculateTotalPrice();
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
                cart.Items.FirstOrDefault(i =>
                    i.ProductId.Equals(ProductId.Create(request.ProductId))
                )?.Quantity
                    ?? 0,
                totalPrice
            )
        );
    }
}
