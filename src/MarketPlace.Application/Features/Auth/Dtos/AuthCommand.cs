namespace MarketPlace.Application.Features.Auth.Dtos;


public sealed record RegisterUserCommandData(string? FirstName,string? LastName, string Email, string Password  );

public sealed record LoginUserCommandData(string Email, string Password);
        
public sealed record RefreshTokenCommandData(string RefreshToken, string? IpAddress);

public sealed record LogoutCommandData(string RefreshToken, string? IpAddress);

public sealed record ForgotPasswordCommandData(string Email);

public sealed record ResetPasswordCommandData(string Email, string Token, string NewPassword);

