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
    public static BaseError PaymentGatewayError()
    {
        return BaseError.BadRequest("Payment.GatewayError", $"Payment gateway error: ");
    }
    public static BaseError PaymentAlreadyPaid()
    {
        return BaseError.BadRequest("Payment.AlreadyPaid", "The payment has already been completed.");
    }
    public static BaseError PaymentMethodNotSupported()
    {
        return BaseError.BadRequest("Payment.MethodNotSupported", "The payment method is not supported.");
    }
    public static BaseError PaymentNotRefunded()
    {
        return BaseError.BadRequest("Payment.NotRefunded", "The payment could not be refunded.");
    }
}
