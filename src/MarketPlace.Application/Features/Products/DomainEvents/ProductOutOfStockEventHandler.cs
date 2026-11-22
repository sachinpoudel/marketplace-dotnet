using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Products.Events;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Products;

public sealed class ProductOutOfStockEventHandler(IVendorRepository vendorRepository)
    : INotificationHandler<ProductOutOfStockEvent>
{
    public async Task Handle(
        ProductOutOfStockEvent notification,
        CancellationToken cancellationToken
    )
    {
        var vendor = await vendorRepository.GetByIdAsync(notification.VendorId, cancellationToken);
        if (vendor is null)
            throw new InvalidOperationException("Vendor not found for this product.");

        vendor.HandleOutOfStock(notification.ProductId);
        await vendorRepository.UpdateAsync(vendor, cancellationToken);
    }
}
