using System.Text.Json.Serialization;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Enums;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentVerifyResponse(
    bool IsSuccess,
    [property: JsonPropertyName("total_amount")] decimal Amount,
    [property: JsonPropertyName("transaction_id")] string TransactionId,
    [property: JsonPropertyName("pidx")] string ExternalReferenceId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("status")] GatewayPaymentStatus PaymentStatus
);

