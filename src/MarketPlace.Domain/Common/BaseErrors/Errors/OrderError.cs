using System;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;

public static class OrderError
{
    public static BaseError OrderNotFound()
    {
        return BaseError.NotFound("Order Not Found", "The order you are trying to access does not exist.");
    }
    public static BaseError OrderCancelled()
    {
        return BaseError.Conflict("Order Cancelled", "The order you are trying to pay for has been cancelled.");
    }
    public static BaseError OrderAlreadyPaid()
    {
        return BaseError.Conflict("Order Already Paid", "The order you are trying to pay for has already been paid.");
    }
    public static BaseError InvalidReservation()
    {
        return BaseError.Conflict("Invalid Reservation", "The order reservation is not active and cannot be consumed or released.");
    }   
    public static BaseError OrderAlreadyCancelled()
    {
        return BaseError.Conflict("Order Already Cancelled", "The order you are trying to cancel has already been cancelled.");
    }
    public static BaseError OrderAlreadyShippedOrDelivered()
    {
        return BaseError.Conflict("Order Already Shipped or Delivered", "The order you are trying to cancel has already been shipped or delivered.");
    }
    
}
