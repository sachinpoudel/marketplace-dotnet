using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Orders.Dtos;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Queries.GetMyOrders;

public record class GetMyOrdersQuery
(
    int PageNumber = 1,
    int PageSize = 10
): IRequest<PaginatedList<OrderListItemDto>>;
