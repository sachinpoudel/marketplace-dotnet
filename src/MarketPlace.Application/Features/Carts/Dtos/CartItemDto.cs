namespace MarketPlace.Application.Features.Carts.Dtos;

public record class CartItemDto(
    Guid ProductId,
    int Quantity,
    Guid CartId,
    double PriceAtTheTimeOfAdding
);
