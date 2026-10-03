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
}
