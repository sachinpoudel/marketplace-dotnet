using System;
using MarketPlace.Application.Common.Interfaces.BackgroundJobs;
using MarketPlace.Application.Features.Orders.Jobs;
using MarketPlace.Domain.Orders.Events;
using MediatR;

namespace MarketPlace.Application.Features.Orders.DomainEvents;

public class OrderPlaceEventHandler(IBackgroundJobService backgroundJobService) : INotificationHandler<OrderPlacedEvent>
{
    public Task Handle(OrderPlacedEvent notification, CancellationToken cancellationToken)
    {
        backgroundJobService.Enqueue<IOrderConfirmationEmailJob>(job => job.ExecuteAsync(notification.OrderId.Value)) ; 
        return Task.CompletedTask;
    }
}
