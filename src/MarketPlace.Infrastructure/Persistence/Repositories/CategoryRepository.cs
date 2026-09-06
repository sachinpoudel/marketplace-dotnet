using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(ApplicationDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await context.Categories.AddAsync(category, cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(CategoryId categoryId, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AnyAsync(c => c.ParentCategoryId != null && c.ParentCategoryId == categoryId, cancellationToken);
    }

    public async Task<int> CountValidLeafCategoriesAsync(IEnumerable<CategoryId> categoryIds, CancellationToken cancellationToken = default)
    {
        var ids = categoryIds.Distinct().ToList();
        if (ids.Count == 0)
            return 0;

        var validIds = ids.Select(id => id.Value).ToHashSet();

        return await context.Categories
            .Where(c => validIds.Contains(c.Id.Value))
            .Where(c => !context.Categories.Any(child =>
                child.ParentCategoryId != null &&
                child.ParentCategoryId.Value == c.Id.Value))
            .CountAsync(cancellationToken);
    }
}
