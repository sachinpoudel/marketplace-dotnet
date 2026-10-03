namespace MarketPlace.Application.Features.Carts.Dtos;

public sealed record CartDetailDto (
   Guid CartId,
   Guid UserId,
   List<string> Items,
   decimal TotalPrice
);