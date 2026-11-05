using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;



    public static class ReviewError
{
    public static BaseError ReviewPublishFailed()
    {
        return BaseError.InternalServerError("Review Publish Failed", "The review couldnt be published due to internal reason");
    }
    public static BaseError UserHasAlreadyReviewedProduct()
    {
        return BaseError.BadRequest("User Has Already Reviewed Product", "You cannot review a product you have already reviewed");
    }
    public static BaseError ReviewNotFound()
    {
        return BaseError.NotFound("Review Not Found", "The review you are trying to access does not exist");
    }
    public static BaseError UserHasNotPurchasedProduct()
    {
        return BaseError.BadRequest("User Has Not Purchased Product", "You cannot review a product you have not purchased");
    }
    
}