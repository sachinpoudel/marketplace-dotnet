namespace MarketPlace.Application.Features.Orders.Dtos;

public record class OrderListItemDto
(
    Guid Id,
    double TotalAmount,
    string Status,
    List<OrderItemDto> Items,
    DateTime CreatedAt
);
