using System;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;

public static class OrderError
{
    public static BaseError OrderNotFound()
    {
        return BaseError.NotFound("Order Not Found", "The order you are trying to access does not exist.");
    }
}
