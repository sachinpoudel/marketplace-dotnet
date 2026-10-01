using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Features.Products.Dtos;


public record UpdatedProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    IEnumerable<string> Tags);