using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(Guid productId, CancellationToken cancellationToken = default);
    IQueryable<Product> Query();   
}