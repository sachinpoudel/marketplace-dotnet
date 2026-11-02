using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Create;

public class CreateOrderCommandHandler(
    ICartRepository cartRepository,
    ICurrentUser currentUser,
    IProductRepository productRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateOrderCommand, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();

        if (currentUserId == Guid.Empty)
        {
            return Result<OrderDetailDto>.Failure(UserError.UserNotAuthenticated());
        }

        var cart =
            request.CartId != Guid.Empty
                ? await cartRepository.GetCartByIdAsync(request.CartId, cancellationToken)
                : await cartRepository.GetCartByUserIdAsync(currentUserId, cancellationToken);

        if (cart == null || cart.UserId != currentUserId)
        {
            return Result<OrderDetailDto>.Failure(CartError.CartNotFound());
        }
        if (cart.Items.Count == 0)
        {
            return Result<OrderDetailDto>.Failure(CartError.EmptyCart());
        }

        var productIds = cart.Items.Select(i => i.ProductId).ToList();

        var products = await productRepository.GetByIdsAsync(productIds, cancellationToken); // for checking if all products exist and their status and stock

        if (products.Count() != productIds.Count)
        {
            return Result<OrderDetailDto>.Failure(ProductError.ProductNotFound());
        }
        var shippingAddress = ShippingAddress.Create(
            request.FullName,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.PhoneNumber
        );
        var order = Order.Create(currentUserId, shippingAddress);
        foreach (var cartitem in cart.Items)
        {
            var product = products.FirstOrDefault(p => p.Id == cartitem.ProductId);
            if (product == null)
            {
                return Result<OrderDetailDto>.Failure(ProductError.ProductNotFound());
            }
            var status = product.Status;
            Console.WriteLine($"Product {product.Id.Value} status: {status}"); // Debugging line
            if (status != Domain.Products.Enums.ProductStatus.Available && status != Domain.Products.Enums.ProductStatus.Active)
            {
                Console.WriteLine($"Product {product.Id.Value} is not available or active. Status: {status}"); // Debugging line
                return Result<OrderDetailDto>.Failure(
                    ProductError.ProductUnavailable(product.Id.Value)
                );
            }
            var stock = product?.StockQuantity ?? 0;
            Console.WriteLine($"Product {product.Id.Value} stock: {stock}, requested quantity: {cartitem.Quantity}"); // Debugging line
            if (cartitem.Quantity > stock)
            {
                return Result<OrderDetailDto>.Failure(
                    ProductError.InsufficientStock(product.Id.Value)
                );
            }

            order.AddItem(
                product.Id,
                cartitem.Quantity,
                product.Name,
                product.Price,
                product.VendorId
            );
            product.ReduceStock(cartitem.Quantity);
        }
        await orderRepository.AddAsync(order, cancellationToken);
        await cartRepository.ClearCartAsync(cart.Id, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result<OrderDetailDto>.Success(
            new OrderDetailDto(
                order.Id.Value,
                order.UserId,
                new ShippingAddressDto(
                    order.ShippingAddress.FullName,
                    order.ShippingAddress.AddressLine1,
                    order.ShippingAddress.AddressLine2,
                    order.ShippingAddress.City,
                    order.ShippingAddress.State,
                    order.ShippingAddress.PhoneNumber
                ),
                order.Status.ToString(),
                order
                    .Items.Select(i => new OrderItemDto(
                        i.Id.Value,
                        i.OrderId.Value,
                        i.ProductId.Value,
                        i.ProductName,
                        i.Quantity,
                        i.Price,
                        i.VendorId.Value
                    ))
                    .ToList()
            )
        );
    }
}
