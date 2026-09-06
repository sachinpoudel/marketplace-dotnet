using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
  Task<bool> ExistsAsync(ProductId productId, CancellationToken cancellationToken = default);
    IQueryable<Product> Query();   
}