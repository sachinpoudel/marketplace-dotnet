using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Queries.GetOrderById;

public record class GetOrderByIdQuery
(
Guid OrderId
) : IRequest<Result<OrderDetailDto>>;