using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Features.Products.Dtos;


public sealed record ProductsListItemDto (
ProductId Id,
string Name,
decimal Price,
string ImageUrl,
string Status,
string Description,
string Tags,
string Sku,
int StockQuantity
);