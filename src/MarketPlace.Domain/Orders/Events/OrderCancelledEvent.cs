using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Orders.ValueObjects;

namespace MarketPlace.Domain.Orders.Events;

public record class OrderCancelledEvent(OrderId OrderId, Guid UserId) : IDomainEvent;

