using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Payments.Events;

public sealed record PaymentPaidEvent(Guid PaymentId, Guid OrderId, decimal Amount, string TransactionId, DateTime PaidAt) : IDomainEvent;