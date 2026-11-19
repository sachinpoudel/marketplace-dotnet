using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Command.Refund;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Orders.Events;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Orders;

public class OrderCancelledEventHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ISender sender,
    IPaymentRepository paymentRepository
) : INotificationHandler<OrderCancelledEvent>
{
    public async Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetOrderByIdAsync(
            notification.OrderId,
            cancellationToken
        );
        if (order is null)
        {
            throw new InvalidOperationException("Order not found for this event.");
        }

        foreach (var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
            {
                throw new InvalidOperationException(
                    $"Product with ID {item.ProductId} not found for this order."
                );
            }
            product.ReleaseReservedStock(item.Quantity);
        }

        var payment = await paymentRepository.GetPaymentByOrderIdAsync(order.Id, cancellationToken);
        if (payment is null)
        {
            return;
        }
       switch (payment.Status)
        {
            case PaymentStatus.Pending: 
            payment.MarkAsFailed();
            break;

            case PaymentStatus.Completed:
               payment.MarkAsRefundPending();
               break;
                
        }

      
    }
}
