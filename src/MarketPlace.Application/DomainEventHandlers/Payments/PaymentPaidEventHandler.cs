using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Payments.Events;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Payments;

public sealed class PaymentPaidEventHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository


) : INotificationHandler<PaymentPaidEvent>
{
    public async Task Handle(PaymentPaidEvent notification, CancellationToken cancellationToken)
    {
var order = await orderRepository.GetOrderByIdAsync(notification.OrderId, cancellationToken);

      
             if (order is null)
            throw new InvalidOperationException(
                "Order not found for this payment."
            );

            foreach(var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if(product is null)
                throw new InvalidOperationException(
                    $"Product with ID {item.ProductId} not found for this order."
                );
                var result = product.CommitReservedStock(item.Quantity);
                if(result.IsFailure)
                    throw new InvalidOperationException(
                        $"Failed to commit reserved stock for product with ID {item.ProductId}."
                    );
        }
        order.ConsumeReservation();
            order.Confirm();
        
    }
}
