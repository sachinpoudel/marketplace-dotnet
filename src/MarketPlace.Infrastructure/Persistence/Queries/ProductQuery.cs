using MarketPlace.Application.Common.Interfaces.Query;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace MarketPlace.Infrastructure.Persistence.Queries;


public class ProductQuery(ApplicationDbContext context) : IProductQuery
{
    public async Task<ProductDetailDto?> GetProductDetailAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        return await context.Products
                    .Where(p => p.Id == productId)
                    .Select(p => new ProductDetailDto(
                         p.Id.Value,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.StockQuantity,
                        p.Sku,
                        p.Images.Select(i => i.Url).ToList(),
                        p.Tags.Select(t => t.Name).ToList(),
                        p.CategoryIds.Select(c => c.Value).ToList(),
                        p.VendorId.Value,
                        context.Reviews 
                            .Where(r => r.ProductId == p.Id)
                            .Select(r => new ReviewDetailDto(
                                r.Id.Value,
                                r.ProductId.Value,
                                r.UserId,
                                r.Rating,
                                r.Content,
                                r.CreatedAt
                            )) 
                            .ToList()
                    )).FirstOrDefaultAsync(cancellationToken);
                   
    }
}
