using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
    Task<bool> HasParentAsync(CategoryId categoryId, CancellationToken cancellationToken = default);
}