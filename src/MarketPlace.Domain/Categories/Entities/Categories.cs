using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Domain.Categories.Entities;


public class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;

    public int DisplayOrder { get; private set; } = 0;
    public bool IsActive { get; private set; }
    public Guid? ParentCategoryId { get; private set; } = null;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;



    private Category() { }


    public static Result<Category> Create(string name, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Category>.Failure(CategoryError.CategoryNameIsRequired());

        // if (string.IsNullOrWhiteSpace(slug)) 
        //     return Result<Category>.Failure(CategoryError.SlugIsRequired());

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description,
            ParentCategoryId = parentCategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };



        return Result<Category>.Success(category);
    }


    public Result UpdateDetails(string name, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(CategoryError.CategoryNameIsRequired());

        Name = name.Trim();
        Description = description;
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
    public Result MoveToParentCategory(Guid? newParentCategoryId)
    {
        if (newParentCategoryId == Id)
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