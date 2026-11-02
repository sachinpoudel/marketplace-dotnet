using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Application.Features.Categories.Dtos;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Common.BaseErrors;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.AddItem;

public class AddCartItemCommandHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    IProductRepository productRepository,
    ICurrentUser currentUser
) : IRequestHandler<AddCartItemCommand, Result<CartDetailDto>>
{
    public async Task<Result<CartDetailDto>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken
    )
    {
        var isValidQuantity = request.Quantity > 0 && request.Quantity <= 100;
        if (!isValidQuantity)
        {
            return Result.Failure<CartDetailDto>(CartError.InvalidQuantity());
        }
        // check product exists
        var productId = ProductId.Create(request.ProductId);
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product == null)
        {
            return Result.Failure<CartDetailDto>(ProductError.ProductNotFound());
        }
        var userId = currentUser.GetCurrentUserId();
        if (userId == Guid.Empty)
        {
            return Result.Failure<CartDetailDto>(UserError.UserNotAuthenticated());
        }

        var cart = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);

        var productPrice = product.Price;
        if (cart is null)
        {
             cart = Cart.Create(userId);
            await cartRepository.AddItemToCartAsync(cart, cancellationToken);
        }
            cart.AddItem(productId, request.Quantity, productPrice);
        
        var totalPrice = cart?.CalculateTotalPrice() ?? 0;
        Console.WriteLine($"Total Price: {totalPrice}"); // Debugging line
        var TotalCartQuantity = cart?.CalculateTotalQuantity() ?? 0;
        await unitOfWork.CommitAsync(cancellationToken);

        return Result<CartDetailDto>.Success(
            new CartDetailDto(
                cart.Id.Value,
                cart.UserId,
                cart
                    .Items.Select(i => new CartItemDto(
                        i.ProductId.Value,
                        i.Quantity,
                       
                        i.PriceAtTheTimeOfAdding
                    ))
                    .ToList(),
                TotalCartQuantity,
                totalPrice
            )
        );
    }
}
