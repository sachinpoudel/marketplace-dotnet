namespace MarketPlace.Application.Features.Carts.Dtos;

public sealed record CartDetailDto (
   Guid CartId,
   Guid UserId,
   List<CartItemDto> Items,
   int Quantity,
   double TotalPrice
);