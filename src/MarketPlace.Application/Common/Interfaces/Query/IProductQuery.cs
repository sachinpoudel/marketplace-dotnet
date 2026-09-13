using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Query;


public interface IProductQuery
{
    Task<ProductDetailDto?> GetProductDetailAsync(ProductId productId, CancellationToken cancellationToken = default);
}