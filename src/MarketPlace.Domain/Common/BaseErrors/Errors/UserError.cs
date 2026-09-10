using MarketPlace.Domain.Common.Enums;

namespace MarketPlace.Domain.Common.BaseErrors.Errors;



    public static class UserError
{
    public static BaseError UserNotFound()
    {
        return BaseError.NotFound("User not found.", "The specified user does not exist.");
    }

    public static BaseError UserCreationFailed()
    {
        return BaseError.InternalServerError("User creation failed.", "The user could not be created due to an internal error.");
    }

    public static BaseError InvalidUserCredentials()
    {
        return BaseError.BadRequest("Invalid credentials.", "The provided user credentials are invalid.");
    }
    public static BaseError InvalidRefreshToken()
    {
        return BaseError.BadRequest("Invalid refresh token.", "The provided refresh token is invalid or expired.");
    }
    public static BaseError RefreshTokenRevokedOrExpired()
    {
        return BaseError.BadRequest("Refresh token revoked or expired.", "The provided refresh token has been revoked or has expired.");
    }
}