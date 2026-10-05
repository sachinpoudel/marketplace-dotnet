using System;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;

public interface IOrderRepository
{
  Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
  Task<Order?> GetOrderByIdAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
