using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;



    public static class ProductError
{
    public static BaseError ProductNameIsRequired()
    {
        return BaseError.BadRequest("Product name is required.", "The product name cannot be null or empty.");
    }
}