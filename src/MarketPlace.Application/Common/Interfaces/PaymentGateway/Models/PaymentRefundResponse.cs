using System.Text.Json.Serialization;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentRefundResponse(
    bool IsSuccess,
   [property: JsonPropertyName("refund_transaction_id")] string? RefundTransactionId = null,
    string ErrorMessage = ""
);