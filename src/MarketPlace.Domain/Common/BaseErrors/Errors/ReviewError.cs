using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;



    public static class ReviewError
{
    public static BaseError ReviewPublishFailed()
    {
        return BaseError.InternalServerError("Review Publish Failed", "The review couldnt be published due to internal reason");
    }
    
}