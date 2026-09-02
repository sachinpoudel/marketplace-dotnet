using MarketPlace.Domain.Common.Interfaces;

namespace MarketPlace.Domain.Vendors.Events;

public sealed record VendorRegisteredEvent(Guid VendorId) : IDomainEvent;

public sealed record VendorDelistedEvent(Guid VendorId) : IDomainEvent;

public sealed record VendorAcceptedEvent(Guid VendorId) : IDomainEvent;