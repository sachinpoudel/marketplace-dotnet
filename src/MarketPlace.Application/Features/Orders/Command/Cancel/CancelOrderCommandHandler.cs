using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Cancel;

public class CancelOrderCommandHandler(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IProductRepository productRepository
) : IRequestHandler<CancelOrderCommand, Result<CancelOrderDto>>
{
    public async Task<Result<CancelOrderDto>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<CancelOrderDto>.Failure(UserError.UserNotAuthenticated());
        }

        var order = await orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );
        if (order == null)

        {
            return Result<CancelOrderDto>.Failure(OrderError.OrderNotFound());
        }
        if (order.UserId != currentUserId)
        {
            return Result<CancelOrderDto>.Failure(UserError.UserNotAuthorized());
        }

        order.Cancel();
        foreach (var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product != null)
            {
                product.IncreaseStock(item.Quantity);
            }
        }
        
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<CancelOrderDto>.Success(new CancelOrderDto(Guid.Parse(order.Id.ToString())));
    }
}
