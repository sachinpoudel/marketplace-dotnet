using System;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Orders.Dtos;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Queries.GetMyOrders;

public class GetMyOrdersQueryHandler(ICurrentUser currentUser, IOrderRepository orderRepository) : IRequestHandler<GetMyOrdersQuery, PaginatedList<OrderListItemDto>>
{
    public async Task<PaginatedList<OrderListItemDto>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetCurrentUserId();
        var query =  orderRepository.Query()
             .Where(o => o.UserId == currentUserId)
             .OrderByDescending(o => o.CreatedAt)
             .Select(o => new OrderListItemDto(
                 o.Id.Value,

                 o.TotalAmount,
                 o.Status.ToString(),
                 o.Items.Select(i => new OrderItemDto(
                     i.Id.Value,
                     i.OrderId.Value,
                     i.ProductId.Value,
                     i.ProductName,
                     i.Quantity,
                     i.Price,
                     i.VendorId.Value
                 )).ToList(),
                 o.CreatedAt
             ));

        return await query.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
