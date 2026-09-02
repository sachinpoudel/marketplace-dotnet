using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface ICategoryRepository
{
    Task<Result<Category>> GetByIdAsync(Guid categoryId);
    Task<Result<Category>> AddAsync(Category category);
    Task<Result<Category>> GetByCategoryIdAsync(Guid categoryId);
    
}