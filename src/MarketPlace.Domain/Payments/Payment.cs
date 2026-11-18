using System;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.Exceptions;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MarketPlace.Domain.Payments.Events;
using MarketPlace.Domain.Payments.ValueObjects;

namespace MarketPlace.Domain.Payments;

public class Payment : AggregateRoot<PaymentId>
{
    public OrderId OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string? ExternalReferenceId { get; private set; }
    public string? TransactionId { get; private set; }
    public string? RefundTransactionId { get; private set; }
    public bool IsManual { get; private set; }
    public DateTime RefundAt { get; private set; }
    public DateTime PaidAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payment(
        PaymentId id,
        OrderId orderId,
        Guid userId,
        decimal amount,
        PaymentStatus status,
        PaymentMethod method,
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
        CreatedAt = createdAt;
    }

    public static Payment Create(OrderId orderId, Guid userId, decimal amount, PaymentMethod method)
    {
        return new Payment(
            PaymentId.Create(),
            orderId,
            userId,
            amount,
            PaymentStatus.Pending,
            method,
            DateTime.UtcNow
        );
    }

    public void SetExternalReferenceId(string pidx)
    {
        if (string.IsNullOrWhiteSpace(pidx))
        {
            throw new DomainException("External reference ID cannot be null or empty.");
        }
        ExternalReferenceId = pidx;
    }

    public void MarkAsPaid(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new DomainException("Only pending payments can be marked as paid.");
        }
        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentPaidEvent(Id, OrderId, Amount, TransactionId!, PaidAt));
    }

    public void MarkAsFailed()
    {
        if (Status == PaymentStatus.Completed)
            throw new DomainException("Completed payments cannot be marked as failed.");
        Status = PaymentStatus.Failed;
    }

    public void ResetForRetry(string newReferenceId, PaymentMethod paymentMethod)
    {
        if (Status == PaymentStatus.Completed)
            throw new DomainException("Completed payments cannot be reset for retry.");

        Method = paymentMethod;
        Status = PaymentStatus.Pending;
        ExternalReferenceId = newReferenceId;
    }

    public Result MarkAsRefunded(string? refundTransactionId, bool isManual = false)
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only paid payments can be refunded.");

        Status = PaymentStatus.Refunded;
        RefundTransactionId = refundTransactionId;
        RefundAt = DateTime.UtcNow;
        IsManual = isManual;

        RaiseDomainEvent(
            new PaymentRefundedEvent(Id, OrderId, Amount, RefundTransactionId!, RefundAt, isManual)
        );
        return Result.Success();
    }
}
