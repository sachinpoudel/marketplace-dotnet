using System;
using System.Text.Json.Serialization;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;

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







