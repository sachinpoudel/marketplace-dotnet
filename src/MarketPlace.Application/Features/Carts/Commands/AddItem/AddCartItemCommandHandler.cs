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

public class AddCartItemCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork,IProductRepository productRepository, ICurrentUser currentUser) : IRequestHandler<AddCartItemCommand, Result<CartDetailDto>>
{
    public async Task<Result<CartDetailDto>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
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
        var userId = Guid.Parse(currentUser.UserId);

        var cartByUserId = await cartRepository.GetCartByUserIdAsync(userId, cancellationToken);
        
 if(cartByUserId is null)
        {
            
        var cart = Cart.Create(userId);
        cart.AddItem(productId, request.Quantity);
        await cartRepository.AddItemToCartAsync(cart, cancellationToken);
        }else
        {
            cartByUserId.AddItem(productId, request.Quantity);
        }

        await unitOfWork.CommitAsync(cancellationToken);   
        
        var quantity = cartByUserId.Items.FirstOrDefault(i => i.ProductId.Equals(productId))?.Quantity ?? 0;
        return Result<CartDetailDto>.Success(new CartDetailDto(
            cartByUserId.Id.Value,
            cartByUserId.UserId,
            cartByUserId.Items.Select(i => $"{i.ProductId} - {i.Quantity}").ToList(),
            quantity

        ));
    }


}
