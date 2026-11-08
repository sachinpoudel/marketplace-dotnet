using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Orders.Enums;
using MarketPlace.Domain.Payments.Events;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Payments;

public sealed class PaymentRefundedEventHandler (IOrderRepository orderRepository): INotificationHandler<PaymentRefundedEvent>
{
    public async  Task Handle(PaymentRefundedEvent notification, CancellationToken cancellationToken)
    {
       if (notification.IsManual)
    {
        var order = await orderRepository.GetOrderByIdAsync(notification.OrderId, cancellationToken);
        if (order is not null && order.Status != OrderStatus.Cancelled)
        {
            order.MarkAsRefunded();
        }
    }
    }
}
