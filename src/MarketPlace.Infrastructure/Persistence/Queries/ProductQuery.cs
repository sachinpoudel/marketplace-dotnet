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
        var product = await context.Products
              .AsNoTracking()
              .FirstOrDefaultAsync(
                  p => p.Id == productId,
                  cancellationToken);
      
          if (product is null)
              return null;
      
          var reviews = await context.Reviews
              .AsNoTracking()
              .Where(r => r.ProductId == product.Id)
              .Select(r => new ReviewDetailDto(
                  r.Id.Value,
                  r.ProductId.Value,
                  r.UserId,
                  r.Rating,
                  r.Content,
                  r.CreatedAt
              ))
              .ToListAsync(cancellationToken);
      
          return new ProductDetailDto(
              product.Id.Value,
              product.Name,
              product.Description,
              product.Price,
              product.StockQuantity,
              product.Sku,
              product.Images.Select(i => i.Url).ToList(),
              product.Tags.Select(t => t.Name).ToList(),
              product.CategoryIds.Select(c => c.Value).ToList(),
              product.VendorId.Value,
              reviews
          );                   
    }
}
