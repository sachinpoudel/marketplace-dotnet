using System.Transactions;

namespace MarketPlace.Infrastructure.ExternalServices.Payments.khalti.Models;

public record class KhaltiRefundResponse
(
  string transaction_id
);
