using System;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler(ICurrentUser currentUser, IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();

        var order = orderRepository
            .GetOrderByIdAsync(OrderId.Create(request.OrderId), cancellationToken)
            .Result;
        if (order is null)
            return Result.Failure<OrderDetailDto>(OrderError.OrderNotFound());

        if (order.UserId != currentUserId)
        {
            return Result<OrderDetailDto>.Failure(UserError.UserNotAuthorized());
        }
        return Result<OrderDetailDto>.Success(
            new OrderDetailDto(
                Guid.Parse(order.Id.ToString()),
                Guid.Parse(order.UserId.ToString()),
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
