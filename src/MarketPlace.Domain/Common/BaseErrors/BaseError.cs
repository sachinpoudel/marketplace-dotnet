using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors;

public abstract class BaseError(string title, string message, StatusCode statusCode)
{
    public string Title { get; } = title;
    public string Message { get; } = message;
    public StatusCode StatusCode { get; } = statusCode;

    public static BaseError None() => new GeneralError(string.Empty, string.Empty, StatusCode.InternalServerError);


    public static BaseError BadRequest(string title, string message) => new GeneralError(title, message, StatusCode.BadRequest); 
}