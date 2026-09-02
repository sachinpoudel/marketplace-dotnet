using MarketPlace.Domain.Common.Interfaces;

namespace MarketPlace.Domain.Products.Events;

public sealed record ProductCreatedEvent(Guid ProductId, Guid VendorId) : IDomainEvent;

public sealed record ProductOutOfStockEvent(Guid ProductId) : IDomainEvent;