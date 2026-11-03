using System;
using MarketPlace.Domain.Common.Interfaces;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public class PurchaseVerificationRepository (ApplicationDbContext context): IPurchaseVerification
{
    public async Task<bool> HasUserPurchasedProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return await context.Orders.AnyAsync(o => o.UserId == userId && o.Status == Domain.Orders.Enums.OrderStatus.Confirmed && o.Items.Any(i => i.ProductId == ProductId.Create(productId)), cancellationToken);
    }
}
