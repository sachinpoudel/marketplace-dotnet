namespace MarketPlace.Application.Features.Orders.Dtos;

public record class OrderItemDto
(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    double Price,
    Guid VendorId
);
