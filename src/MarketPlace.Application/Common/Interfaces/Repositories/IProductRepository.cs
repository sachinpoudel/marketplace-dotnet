using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IProductRepository
{
    Task<Result<Product>> GetByIdAsync(Guid productId);
    Task<Result<Product>> AddAsync(Product product);
    Task<Result<Product>> GetByProductIdAsync(Guid productId);
    
}