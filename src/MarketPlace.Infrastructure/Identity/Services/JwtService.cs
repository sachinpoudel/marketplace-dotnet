using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MaketPlace.Application.Features.Auth.Dtos;
using MarketPlace.Infrastructure.Identity.Models;
using MarketPlace.Infrastructure.Persistence.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MarketPlace.Infrastructure.Identity.Services;

public sealed class JwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public AuthSessionData CreateSession(ApplicationUser user, IEnumerable<string> roles, string refreshToken)
    {
        var accessTokenExpiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var accessToken = CreateAccessToken(user, roles, accessTokenExpiresAtUtc);

        return new AuthSessionData(
            user.Id,
            user.Email!,
            accessToken,
            accessTokenExpiresAtUtc,
            refreshToken,
            DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenDays)
            );
    }

    public string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, DateTimeOffset expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public  string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}