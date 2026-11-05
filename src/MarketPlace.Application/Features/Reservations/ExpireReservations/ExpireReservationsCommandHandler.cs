using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Reservations.ExpireReservations;
public class ExpireReservationsCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IProductRepository productRepository)
    : IRequestHandler<ExpireReservationsCommand, Result>
{
    public async Task<Result> Handle(
        ExpireReservationsCommand request,
        CancellationToken cancellationToken)
    {
        var expiredOrders =
            await orderRepository.GetExpiredReservationsAsync(
                DateTime.UtcNow,
                cancellationToken);

        foreach (var order in expiredOrders)
        {
            if (order.ReservationStatus != Domain.Orders.Enums.OrderReservationStatus.Active)
                continue; 

            foreach (var orderItem in order.Items)
            {
                var product = await productRepository.GetByIdAsync(
                    orderItem.ProductId,
                    cancellationToken);

                if (product is null)
                    return Result.Failure(ProductError.ProductNotFound());

                var result = product.ReleaseReservedStock(orderItem.Quantity);

                if (result.IsFailure)
                    return Result.Failure(
                        ProductError.ProductStockReleaseFailed());
            }

            var releaseResult = order.ReleaseReservation();

            if (releaseResult.IsFailure)
                return Result.Failure(releaseResult.Error);

            var cancelResult = order.Cancel();

            if (cancelResult.IsFailure)
                return Result.Failure(cancelResult.Error);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
