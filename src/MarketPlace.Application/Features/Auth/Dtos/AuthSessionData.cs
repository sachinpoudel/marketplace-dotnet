namespace MaketPlace.Application.Features.Auth.Dtos;


public record AuthSessionData(
Guid UserId,
string Email,
    string AccessToken,
DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
DateTimeOffset RefreshTokenExpiresAtUtc
    );
