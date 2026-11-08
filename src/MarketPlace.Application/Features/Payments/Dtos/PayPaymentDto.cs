using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MarketPlace.Domain.Payments.ValueObjects;

namespace MarketPlace.Application.Features.Payments.Dtos;

public record class PayPaymentDto
(

    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    PaymentStatus Status,
    PaymentMethod Method,
    DateTime PaidAt,
    string? TransactionId
);
