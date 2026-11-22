using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Payments.Events;
using MediatR;

namespace MarketPlace.Application.DomainEventHandlers.Payments;

public sealed class PaymentPaidEventHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    ICartRepository cartRepository
) : INotificationHandler<PaymentPaidEvent>
{
    public async Task Handle(PaymentPaidEvent notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetOrderByIdAsync(
            notification.OrderId,
            cancellationToken
        );

        if (order is null)
            throw new InvalidOperationException("Order not found for this payment.");

        foreach (var item in order.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                throw new InvalidOperationException(
                    $"Product with ID {item.ProductId} not found for this order."
                );
            var result = product.CommitReservedStock(item.Quantity);
            if (result.IsFailure)
                throw new InvalidOperationException(
                    $"Failed to commit reserved stock for product with ID {item.ProductId}."
                );

            var cart = await cartRepository.GetCartByUserIdAsync(order.UserId, cancellationToken);
            if (cart is null)
                throw new InvalidOperationException(
                    $"Cart not found for user with ID {order.UserId}."
                );
            if (cart.Items.Any(i => i.ProductId == item.ProductId))
            {
                cart.ClearCart();
            }
        }
        order.ConsumeReservation();
        order.Confirm();
    }
}
