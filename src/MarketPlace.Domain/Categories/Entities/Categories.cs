using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.ValueObjects;

namespace MarketPlace.Domain.Categories.Entities;

    
public class Category : AggregateRoot<CategoryId>
{
  private readonly List<CategoryId> _children = new();
        private readonly List<ProductId> _productIds = new();


    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; }
    public CategoryId ParentCategoryId { get; private set; } = null;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

     public IReadOnlyList<CategoryId> Children => _children.AsReadOnly();
        public IReadOnlyList<ProductId> ProductIds => _productIds.AsReadOnly();

    private Category() { }

private Category(
        CategoryId id,
        string name,
        string? description,
        CategoryId? parentCategoryId,
        List<CategoryId> children    ) : base(id)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId ?? null;
        IsActive = true;
        _children = children;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }


    public static Result<Category> Create(string name, string? description = null, CategoryId? parentCategoryId = null, List<CategoryId> children = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Category>.Failure(CategoryError.CategoryNameIsRequired());

        // if (string.IsNullOrWhiteSpace(slug)) 
        //     return Result<Category>.Failure(CategoryError.SlugIsRequired());

        var category = new Category
        {
            Id = CategoryId.Create(),
            Name = name.Trim(),
            Description = description,
            ParentCategoryId = parentCategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };



        return Result<Category>.Success(category);
    }


    public Result UpdateDetails(string name, string? description = null, CategoryId? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(CategoryError.CategoryNameIsRequired());

        Name = name.Trim();
        Description = description;
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
    public Result MoveToParentCategory(CategoryId? newParentCategoryId)
    {
        if (newParentCategoryId.Equals(Id))
            return Result.Failure(CategoryError.CategoryCannotBeOwnParent());


        ParentCategoryId = newParentCategoryId;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
    public Result Activate()
    {
        if (IsActive)
            return Result.Failure(CategoryError.CategoryIsAlreadyActive());

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

}