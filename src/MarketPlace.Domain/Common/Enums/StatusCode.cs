namespace MarketPlace.Domain.Common.Enums;

public enum StatusCode
{
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,

    Validation = 422,
    ServiceUnavailable = 503}