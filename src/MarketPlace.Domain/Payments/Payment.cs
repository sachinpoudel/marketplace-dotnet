using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.Exceptions;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MarketPlace.Domain.Payments.ValueObjects;

namespace MarketPlace.Domain.Payments;

public class Payment : AggregateRoot<PaymentId>
{
    public OrderId OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public double Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payment(
        PaymentId id,
        OrderId orderId,
        Guid userId,
        double amount,
        PaymentStatus status,
        PaymentMethod method,
        DateTime paidAt,
        DateTime createdAt
    )
        : base(id)
    {
        Id = id;
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
        Status = status;
        Method = method;
        PaidAt = paidAt;
        CreatedAt = createdAt;
    }

    public static Payment Create(
        OrderId orderId,
        Guid userId,
        double amount,

        PaymentMethod method,
        DateTime paidAt
    )
    {
        return new Payment(
            PaymentId.Create(),
            orderId,
            userId,
            amount,
            PaymentStatus.Pending,
            method,
            paidAt,
            DateTime.UtcNow
        );
    }

    public void MarkAsPaid( )
    {
        Status = PaymentStatus.Completed;
         TransactionId = $"MOCK-{Guid.NewGuid()}";
         Status = PaymentStatus.Completed;
       
    }

    public void MarkAsFailed()
    {
        Status = PaymentStatus.Failed;
        
    }

    public void MarkAsRefunded()
    {
           if (Status != PaymentStatus.Completed)
        throw new DomainException("Only paid payments can be refunded.");

        Status = PaymentStatus.Refunded;
      
    }
}
