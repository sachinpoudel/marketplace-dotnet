using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Features.Products.Dtos;


public sealed record ProductsListItemDto (
Guid Id,
string Name,
decimal Price,
IEnumerable<string> ImageUrl,
string Status,
string Description,
IEnumerable<string> Tags,
string Sku,
int StockQuantity
);