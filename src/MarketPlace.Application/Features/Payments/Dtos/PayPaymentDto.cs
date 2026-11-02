using MarketPlace.Domain.Payments.Enums;

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
