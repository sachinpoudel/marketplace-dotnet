using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.ValueObjects;

namespace MarketPlace.Domain.Payments.Events;

public sealed record PaymentPaidEvent(PaymentId PaymentId, OrderId OrderId, decimal Amount, string TransactionId, DateTime PaidAt) : IDomainEvent;