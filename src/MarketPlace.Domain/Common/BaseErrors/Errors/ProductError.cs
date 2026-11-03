using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;

public static class ProductError
{
    public static BaseError ProductNameIsRequired()
    {
        return BaseError.BadRequest(
            "Product name is required.",
            "The product name cannot be null or empty."
        );
    }

    public static BaseError ProductCreationFailed()
    {
        return BaseError.InternalServerError(
            "Product creation failed.",
            "The product could not be created due to an internal error."
        );
    }

    public static BaseError ProductNotFound()
    {
        return BaseError.NotFound(
            "Product not found.",
            "The product with the specified ID does not exist."
        );
    }

    public static BaseError ProductAlreadyDeleted()
    {
        return BaseError.BadRequest(
            "Product already deleted.",
            "The product has already been marked as deleted."
        );
    }

    public static BaseError InsufficientStock(Guid productId)
    {
        return BaseError.BadRequest(
            "Insufficient stock.",
            $"The product with ID {productId} does not have enough stock to fulfill the order."
        );
    }

    public static BaseError ProductUnavailable(Guid productId)
    {
        return BaseError.BadRequest(
            "Product Unavailable.",
            $"The product with ID {productId} is unavailable to fulfill the order."
        );
    }
    public static BaseError ProductNotOwnedByUser()
    {
        return BaseError.Unauthorized(
            "Product not owned by user.",
            "The product is not owned by the current user."
        );
    }
}
