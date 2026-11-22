using System;

namespace MarketPlace.Application.Features.Orders.Jobs;

public interface IOrderConfirmationEmailJob
{
  Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}
