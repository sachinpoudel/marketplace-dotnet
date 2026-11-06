using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Carts.Commands.ClearCart;


public class ClearCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : IRequestHandler<ClearCartCommand, Result>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        // Implementation for clearing the cart
        var cart = await cartRepository.GetCartByIdAsync(CartId.Create(request.CartId), cancellationToken);
        if(cart is null)
        {
            return Result.Failure(CartError.CartNotFound());
        }

        cart.ClearCart();
        await cartRepository.UpdateAsync(cart, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
