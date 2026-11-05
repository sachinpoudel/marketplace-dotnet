using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;

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
        var orderIdValue = OrderId.Create(orderId.Value);
        return await context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderIdValue);
    }

    public IQueryable<Order> Query()
    {
        return context.Orders.AsQueryable();
    }
    public async Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await context.Orders.AnyAsync(o => o.UserId == userId && o.Status == Domain.Orders.Enums.OrderStatus.Delivered && o.Items.Any(i => i.ProductId == ProductId.Create(productId)), cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetExpiredReservationsAsync(DateTime expirationTime, CancellationToken cancellationToken = default)
    {
  return await context.Orders
        .Include(o => o.Items)
        .Where(o =>
            o.ReservationStatus == Domain.Orders.Enums.OrderReservationStatus.Active &&
            o.ReservationExpiresAt <= expirationTime).Take(50).AsSplitQuery()
        .ToListAsync(cancellationToken);    }
}
