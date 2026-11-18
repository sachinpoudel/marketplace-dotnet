namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentInitiateRequest(
    Guid OrderId,
    decimal Amount,
    string CustomerName,
    string CustomerPhone
);