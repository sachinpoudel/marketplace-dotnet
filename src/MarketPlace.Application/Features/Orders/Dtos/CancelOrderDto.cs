using MarketPlace.Domain.Orders.Enums;

namespace MarketPlace.Application.Features.Orders.Dtos;

public record class CancelOrderDto
(
    Guid OrderId,
    OrderStatus OrderStatus,
    DateTime CancelledAt
);