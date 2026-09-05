using MarketPlace.Domain.Common.Interfaces;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Domain.Vendors.Events;

public sealed record VendorRegisteredEvent(VendorId VendorId) : IDomainEvent;

public sealed record VendorDelistedEvent(VendorId VendorId) : IDomainEvent;

public sealed record VendorAcceptedEvent(VendorId VendorId) : IDomainEvent;