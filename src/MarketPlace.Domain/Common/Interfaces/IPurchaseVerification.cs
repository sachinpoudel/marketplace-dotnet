using System;

namespace MarketPlace.Domain.Common.Interfaces;

public interface IPurchaseVerification
{
Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
}
