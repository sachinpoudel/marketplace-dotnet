using System;
using System.Text.Json;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti.Models;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.ExternalServices.Payments.khalti;

public class KhaltiGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly KhaltiSettings _settings; // Injected via IOptions

    public KhaltiGateway(HttpClient httpClient, IOptions<KhaltiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
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

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Key", _settings.SecretKey);

        var response = await _httpClient.PostAsync(
            "epayment/initiate/",
            content,
            cancellationToken
        );
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
        var payload = new
        {
            transaction_id = request.TransactionId,
            total_amount = (int)(request.Amount * 100), // Khalti expects Paisa (Rs * 100)
            // mobile_number = request.MobileNumber
        };
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Key", _settings.SecretKey);
        var resp = await _httpClient.PostAsync("epayment/refund/", content, cancellationToken);
        if (!resp.IsSuccessStatusCode)
        {
            return new PaymentRefundResponse(false, null, "Failed to connect to Khalti");
        }
        var respContent = await resp.Content.ReadAsStringAsync();
        var khaltiResponse = JsonSerializer.Deserialize<KhaltiRefundResponse>(respContent);
        if (khaltiResponse == null || string.IsNullOrEmpty(khaltiResponse.transaction_id))
        {
            return new PaymentRefundResponse(false, null, "Failed to parse Khalti response");
        }
        return new PaymentRefundResponse(true, khaltiResponse.transaction_id);
    }

    public async Task<PaymentVerifyResponse> VerifyPaymentAsync(
        PaymentVerifyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new { pidx = request.ExternalReferenceId }),
            System.Text.Encoding.UTF8,
            "application/json"
        );
        Console.WriteLine($"Khalti Verify Payment Request: {JsonSerializer.Serialize(request)}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Key", _settings.SecretKey);
        var resp = await _httpClient.PostAsync("epayment/lookup/", content, cancellationToken);
        Console.WriteLine($"Khalti Verify Payment Response Status: {resp.StatusCode}");
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Khalti Verify Payment Response: Failed to connect to Khalti");
            return new PaymentVerifyResponse(false, 0, null, "Failed to connect to Khalti");
        }
        var responseContent = await resp.Content.ReadAsStringAsync();
        var khaltiResponse = JsonSerializer.Deserialize<KhaltiLookupResponse>(responseContent);
        Console.WriteLine(
            $"Khalti Verify Payment Response: {JsonSerializer.Serialize(khaltiResponse)}"
        );
        if (khaltiResponse == null || khaltiResponse.status != "Completed")
        {
            return new PaymentVerifyResponse(false, 0, null, "Payment not completed");
        }
        decimal amount = khaltiResponse.total_amount / 100; // Convert back to Rs
        return new PaymentVerifyResponse(
            true,
            amount,
            khaltiResponse.transaction_id,
            khaltiResponse.pidx
        );
    }
}
