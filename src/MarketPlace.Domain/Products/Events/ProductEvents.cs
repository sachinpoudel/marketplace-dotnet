using MarketPlace.Domain.Common.Interfaces;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Domain.Products.Events;

public sealed record ProductCreatedEvent(ProductId ProductId, VendorId VendorId) : IDomainEvent;

public sealed record ProductOutOfStockEvent(ProductId ProductId) : IDomainEvent;