using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        var orders =  await context.Orders.AddAsync(order, cancellationToken);
        return orders.Entity;
    }

    public async  Task<Order?> GetOrderByIdAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        return await context.Orders.FindAsync(orderId, cancellationToken);
    }

    public IQueryable<Order> Query()
    {
        return context.Orders.AsQueryable();
    }
}
