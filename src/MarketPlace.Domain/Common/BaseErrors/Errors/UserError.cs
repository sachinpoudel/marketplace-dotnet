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
        return BaseError.InternalServerError(
            "User creation failed.",
            "An error occurred while creating the user."
        );
    }

    public static BaseError InvalidUserCredentials()
    {
        return BaseError.BadRequest(
            "Invalid credentials.",
            "The provided user credentials are invalid."
        );
    }

    public static BaseError InvalidRefreshToken()
    {
        return BaseError.BadRequest(
            "Invalid refresh token.",
            "The provided refresh token is invalid or expired."
        );
    }

    public static BaseError RefreshTokenRevokedOrExpired()
    {
        return BaseError.BadRequest(
            "Refresh token revoked or expired.",
            "The provided refresh token has been revoked or has expired."
        );
    }

    public static BaseError UserNotAuthenticated()
    {
        return BaseError.BadRequest("User not authenticated.", "The user is not authenticated.");
    }

    public static BaseError InvalidUserId()
    {
        return BaseError.BadRequest("Invalid user ID.", "The provided user ID is not valid.");
    }

    public static BaseError UserNotAuthorized()
    {
        return BaseError.Forbidden(
            "User not authorized.",
            "The user does not have the necessary permissions to perform this action."
        );
    }

    public static BaseError EmailAlreadyExists() =>
        BaseError.Conflict(
            "Email already exists.",
            "The provided email address is already associated with another account."
        );

    public static BaseError IdentityValidationFailed(string message) =>
        BaseError.Validation("Registration validation failed.", message);

    public static BaseError IdentityOperationFailed() =>
        BaseError.InternalServerError(
            "Identity operation failed.",
            "An unexpected error occurred while performing an identity operation."
        );
        public static BaseError SessionCreationFailed() =>
        BaseError.InternalServerError(
            "Session creation failed.",
            "An unexpected error occurred while creating a session."
        );
        public static BaseError RoleAssignmentFailed() =>
        BaseError.InternalServerError(
            "Role assignment failed.",
            "An unexpected error occurred while assigning a role to the user."
        );
}
