using System;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;

public static class PaymentError
{
    public static BaseError PaymentNotFound()
    {
        return BaseError.NotFound("Payment.NotFound", "The payment was not found.");
    }
    public static BaseError PaymentNotPending()
    {
        return BaseError.BadRequest("Payment.NotPending", "The payment is not in a pending state.");
    }
}
