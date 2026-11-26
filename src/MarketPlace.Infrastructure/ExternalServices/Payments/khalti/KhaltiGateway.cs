using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Enums;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti.Models;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.ExternalServices.Payments.khalti;

public class KhaltiGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly KhaltiSettings _settings; // Injected via IOptions
    private readonly HttpClient _lookupHttpClient;

    public KhaltiGateway(HttpClient httpClient, IHttpClientFactory httpClientFactory, IOptions<KhaltiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _lookupHttpClient = httpClientFactory.CreateClient("KhaltiLookup");
    }

    public async Task<PaymentInitiateResponse> InitiatePaymentAsync(
        PaymentInitiateRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var khaltiPayload = new
        {
            return_url = _settings.ReturnUrl,
            website_url = _settings.WebsiteUrl,
            amount = (int)(request.Amount * 100), // Khalti expects Paisa (Rs * 100)
            purchase_order_id = request.OrderId,
            purchase_order_name = $"Order #{request.OrderId}",
            customer_info = new
            {
                name = request.CustomerName,

                phone = request.CustomerPhone,
            },
        };

        var content = new StringContent(
            JsonSerializer.Serialize(khaltiPayload),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "epayment/lookup/");

        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            "Key",
            _settings.SecretKey
        );

        httpRequest.Content = content;

        var response = await _lookupHttpClient.SendAsync(httpRequest, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return new PaymentInitiateResponse(false, null, null, "Failed to connect to Khalti");
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        var khaltiResponse = JsonSerializer.Deserialize<KhaltiInitiateResponse>(responseContent);

        if (
            khaltiResponse is null
            || string.IsNullOrWhiteSpace(khaltiResponse.pidx)
            || string.IsNullOrWhiteSpace(khaltiResponse.payment_url)
        )
        {
            return new PaymentInitiateResponse(
                false,
                null,
                null,
                "Invalid Khalti initiation response"
            );
        }
        return new PaymentInitiateResponse(
            true,
            khaltiResponse?.payment_url ?? string.Empty,
            khaltiResponse?.pidx ?? string.Empty
        );
    }

    public async Task<PaymentRefundResponse> RefundPaymentAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken = default
    )
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            $"merchant-transaction/{Uri.EscapeDataString(request.TransactionId)}/refund/"
        );

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Key",
            _settings.SecretKey
        );

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new PaymentRefundResponse(
                false,
                null,
                $"Failed to connect to Khalti: {response.ReasonPhrase}"
            );
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var khaltiResponse = JsonSerializer.Deserialize<KhaltiRefundResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (khaltiResponse == null)
        {
            return new PaymentRefundResponse(false, null, responseContent);
        }

        return new PaymentRefundResponse(true, khaltiResponse?.transaction_id ?? null, null);
    }

    public async Task<PaymentVerifyResponse> VerifyPaymentAsync(
        PaymentVerifyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new { pidx = request.ExternalReferenceId }),
            Encoding.UTF8,
            "application/json"
        );


       using  var  httpRequest = new HttpRequestMessage(HttpMethod.Post, "epayment/lookup/");

   httpRequest.Headers.Authorization = new AuthenticationHeaderValue(
            "Key",
            _settings.SecretKey
        );
        httpRequest.Content = content;

        var resp = await _httpClient.SendAsync(httpRequest, cancellationToken);
        if(!resp.IsSuccessStatusCode)
        {
            return new PaymentVerifyResponse(
                false,
                0,
                null,
                null,
                $"Failed to connect to Khalti: {resp.ReasonPhrase}",
                GatewayPaymentStatus.Unknown
            );
        }

        var responseContent = await resp.Content.ReadAsStringAsync(cancellationToken);

        var khaltiResponse = JsonSerializer.Deserialize<KhaltiLookupResponse>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (khaltiResponse == null)
        {
            return new PaymentVerifyResponse(
                false,
                0,
                null,
                null,
                "Invalid response from Khalti.",
                GatewayPaymentStatus.Unknown
            );
        }

        var status = khaltiResponse.status?.ToLowerInvariant() switch
        {
            "completed" => GatewayPaymentStatus.Completed,
            "pending" or "initiated" => GatewayPaymentStatus.Pending,
            "expired" => GatewayPaymentStatus.Expired,
            "user canceled" => GatewayPaymentStatus.Failed,
            "refunded" => GatewayPaymentStatus.Refunded,
            _ => GatewayPaymentStatus.Unknown,
        };

        decimal amount = khaltiResponse.total_amount / 100m;

        return new PaymentVerifyResponse(
            true,
            amount,
            khaltiResponse.transaction_id,
            khaltiResponse.pidx,
            null,
            status
        );
    }
}
