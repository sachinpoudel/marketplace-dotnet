namespace MarketPlace.Infrastructure.ExternalServices.Payments.khalti.Models;

public record KhaltiLookupResponse(string pidx, decimal total_amount, string status, string transaction_id);