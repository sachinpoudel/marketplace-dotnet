using System.Text.Json.Serialization;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

public record PaymentVerifyRequest([property: JsonPropertyName("pidx")] string ExternalReferenceId);

