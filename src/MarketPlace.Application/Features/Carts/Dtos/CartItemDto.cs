namespace MarketPlace.Application.Features.Carts.Dtos;

public record class CartItemDto(
    Guid ProductId,
    int Quantity,
   
    decimal PriceAtTheTimeOfAdding
);
