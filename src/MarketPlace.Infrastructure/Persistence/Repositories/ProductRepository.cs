using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Products.Entities;

namespace MarketPlace.Infrastructure.Persistence.Repositories;


public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        // await context.Products.AddAsync(product, cancellationToken);
        // return product;
        throw new NotImplementedException();

    }

    public Task<bool> ExistsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Product> Query()
    {
        throw new NotImplementedException();
    }
}