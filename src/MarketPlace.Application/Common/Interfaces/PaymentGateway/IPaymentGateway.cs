using System;
using System.Text.Json.Serialization;

namespace MarketPlace.Application.Common.Interfaces.PaymentGateway;

public interface IPaymentGateway
{
    Task<PaymentInitiateResponse> InitiatePaymentAsync(
        PaymentInitiateRequest request,
        CancellationToken cancellationToken = default
    );
    Task<PaymentVerifyResponse> VerifyPaymentAsync(
        PaymentVerifyRequest request,
        CancellationToken cancellationToken = default
    );
    Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default
    );
}

public record PaymentInitiateRequest(
    Guid OrderId,
    decimal Amount,
    string CustomerName,
    string CustomerPhone
);

public record PaymentVerifyRequest([property: JsonPropertyName("pidx")] string ExternalReferenceId);

public record PaymentInitiateResponse(
    bool IsSuccess,
    [property: JsonPropertyName("payment_url")] string PaymentUrl,
    [property: JsonPropertyName("pidx")] string ExternalReferenceId,
    string ErrorMessage = ""
);

public record PaymentVerifyResponse(
    bool IsSuccess,
    [property: JsonPropertyName("total_amount")] decimal Amount,
    [property: JsonPropertyName("transaction_id")] string TransactionId,
    [property: JsonPropertyName("pidx")] string ExternalReferenceId
);
public record PaymentRefundRequest(
  [property: JsonPropertyName("transaction_id")]  string TransactionId,
    [property: JsonPropertyName("total_amount")] decimal Amount
    // [property: JsonPropertyName("mobile_number")] string MobileNumber
);

public record PaymentRefundResponse(
    bool IsSuccess,
    [property: JsonPropertyName("refund_transaction_id")] string RefundTransactionId,
    string ErrorMessage = ""
);
