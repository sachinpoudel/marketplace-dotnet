using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors;

public class GeneralError(string title , string message, StatusCode statusCode) : BaseError(title, message, statusCode);