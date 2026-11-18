using System.Text.Json.Serialization;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentInitiateResponse(
    bool IsSuccess,
    [property: JsonPropertyName("payment_url")] string PaymentUrl,
    [property: JsonPropertyName("pidx")] string ExternalReferenceId,
    string ErrorMessage = ""
);