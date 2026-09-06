using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;


public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
        return product;
      

    }

    public  async Task<bool> ExistsAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        return await context.Products.AnyAsync(p => p.Id.Equals(productId), cancellationToken);
       
    }

    public async Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id.Equals(productId), cancellationToken);
    }

    public IQueryable<Product> Query()
    {
        return context.Products.AsQueryable(); 
    }
}