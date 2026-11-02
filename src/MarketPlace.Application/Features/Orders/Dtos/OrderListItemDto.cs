namespace MarketPlace.Application.Features.Orders.Dtos;

public record class OrderListItemDto
(
    Guid Id,
    decimal TotalAmount,
    string Status,
    List<OrderItemDto> Items,
    DateTime CreatedAt
);
