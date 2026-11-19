using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Command.Refund;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Orders.Events;
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
        if (order.Status == Domain.Orders.Enums.OrderStatus.Pending)
        {
            payment.MarkAsFailed();
        }

        if(payment.Status == Domain.Payments.Enums.PaymentStatus.Completed)
        {
            await sender.Send(
                new RefundPaymentCommand(
                    order.Id.Value,
                    payment.Id.Value
                ), cancellationToken
            );
        }
    }
}
