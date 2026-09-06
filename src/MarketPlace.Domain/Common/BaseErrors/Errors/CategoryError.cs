using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;



    public static class CategoryError
{
    public static BaseError CategoryNameIsRequired()
    {
        return BaseError.BadRequest("Category name is required.", "The category name cannot be null or empty.");
    }
    public static BaseError CategoryCannotBeOwnParent()
    {
        return BaseError.BadRequest("Category cannot be its own parent.", "A category cannot be its own parent.");
    }
    public static BaseError CategoryIsAlreadyActive()
    {
        return BaseError.BadRequest("Category is already active.", "The category is already active.");
    }
    public static BaseError CategoryDoesNotExist()
    {
        return BaseError.BadRequest("Category does not exist.", "The specified category does not exist.");
    }
    public static BaseError CategoryMustBeLeaf()
    {
        return BaseError.BadRequest("Category must be a leaf category.", "The specified category must be a leaf category (cannot have subcategories).");
    }
    public static BaseError ParentCategoryDoesNotExist()
    {
        return BaseError.BadRequest("Parent category does not exist.", "The specified parent category does not exist    .");
    }
    public static BaseError InvalidOrNonLeafCategories()
    {
        return BaseError.BadRequest("Invalid or non-leaf categories.", "One or more of the specified categories are invalid or not leaf categories.");
    }
}