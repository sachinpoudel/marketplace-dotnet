using System.Text.Json.Serialization;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentRefundRequest(
    [property: JsonPropertyName("transaction_id")] string TransactionId,
    [property: JsonPropertyName("total_amount")] decimal Amount
// [property: JsonPropertyName("mobile_number")] string MobileNumber
);
