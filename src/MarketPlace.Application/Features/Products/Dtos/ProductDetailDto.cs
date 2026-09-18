using MarketPlace.Application.Features.Reviews.Dtos;

namespace MarketPlace.Application.Features.Products.Dtos;


public record  ProductDetailDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string Sku,
    List<string> ImageUrl,
    List<string> Tags,
    List<Guid> CategoryIds,
    Guid VendorId,
    List<ReviewDetailDto> Reviews
);
