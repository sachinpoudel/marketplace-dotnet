using System;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;

public static class CartError
{
  public static BaseError InvalidQuantity()
    {
        return BaseError.BadRequest("Invalid quantity.", "The quantity must be greater than zero and less than or equal to 100.");
    }
    public static BaseError FailedToAddItem()
    {
        return BaseError.BadRequest("Failed to add item to cart.", "An error occurred while trying to add the item to the cart.");
    }
    public static BaseError CartNotFound()
    {
        return BaseError.NotFound("Cart not found.", "The cart for the specified user was not found.");
    }
    public static BaseError EmptyCart()
    {
        return BaseError.BadRequest("Empty cart.", "The cart is empty. Please add items to the cart before proceeding.");
    }
}
