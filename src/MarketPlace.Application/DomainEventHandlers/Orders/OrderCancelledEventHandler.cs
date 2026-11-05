using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Orders.Events;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Orders;

public class OrderCancelledEventHandler(IOrderRepository orderRepository): INotificationHandler<OrderCancelledEvent>
{
    public Task Handle(OrderCancelledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Order with ID {notification.OrderId} has been cancelled by user {notification.UserId}.");
       return Task.CompletedTask;

    }
}
