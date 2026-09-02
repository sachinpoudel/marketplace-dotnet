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
}