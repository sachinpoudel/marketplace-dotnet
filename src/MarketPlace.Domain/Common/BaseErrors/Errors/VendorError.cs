using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;


    public static class VendorError
{
    public static BaseError VendorNameIsRequired()
    {
        return BaseError.BadRequest("Vendor name is required.", "The vendor name cannot be null or empty.");
    }
    public static BaseError VendorContactEmailIsInvalid()
    {
        return BaseError.BadRequest("Vendor contact email is invalid.", "The vendor contact email must be a valid email address.");
    }

    public static BaseError VendorProfileUrlIsInvalid()
    {
        return BaseError.BadRequest("Vendor profile URL is invalid.", "The vendor profile URL must be a valid URL.");
    }
    public static BaseError InvalidStatusTransition()
    {
        return BaseError.BadRequest("Invalid status transition.", "The vendor status transition is not allowed.");
    }
    public static BaseError VendorNotFoundOrInactive()
    {
        return BaseError.NotFound("Vendor not found.", "The specified vendor does not exist.");
    }
    public static BaseError VendorAlreadyExists()
    {
        return BaseError.Conflict("Vendor already exists.", "A vendor with the same legal name, trade name, and contact email already exists.");
    }
}