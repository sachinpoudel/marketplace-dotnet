using MarketPlace.Domain.Common.BaseErrors;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Infrastructure.Identity.Mappers;

public static class IdentityErrorMapper
{
    public static BaseError Map(IEnumerable<IdentityError> errors)
    {
        var identityErrors = errors.ToList();

        if (identityErrors.Any(e =>
            e.Code is "DuplicateEmail" or "DuplicateUserName"))
        {
            return UserError.EmailAlreadyExists();
        }

        var messages = identityErrors.Select(error =>
            error.Code switch
            {
                "PasswordTooShort" =>
                    "Password must be at least 6 characters long.",

                "PasswordRequiresDigit" =>
                    "Password must contain at least one digit.",

                "PasswordRequiresUpper" =>
                    "Password must contain at least one uppercase letter.",

                "PasswordRequiresLower" =>
                    "Password must contain at least one lowercase letter.",

                "PasswordRequiresNonAlphanumeric" =>
                    "Password must contain at least one special character.",

                "InvalidEmail" =>
                    "Please enter a valid email address.",
                    "TokenInvalid" =>
                    "The provided token is invalid or has expired.",

                _ => null
            })
            .Where(message => message != null)
            .Distinct();

        var message = string.Join(" ", messages);

        if (!string.IsNullOrEmpty(message))
            return UserError.IdentityValidationFailed(message);

        return UserError.IdentityOperationFailed();
    }
}