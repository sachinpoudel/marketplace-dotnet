namespace MarketPlace.Infrastructure.Identity.Models;


public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? CreatedByIp { get; set; } = null!;
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    // Foreign key to ApplicationUser
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;


 public static RefreshToken Create(Guid UserId , string tokenHash , string? ipAddress ,DateTime expiresAt)
    {
        return new RefreshToken
        {
            ApplicationUserId = UserId,
            Token = tokenHash,
            Expires = expiresAt,
            CreatedByIp = ipAddress

                    };
    }
    public static RefreshToken Revoke(string ipAddress, RefreshToken refreshToken, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = replacedByToken;

        return refreshToken;
    }
    public  RefreshToken Rotate( RefreshToken refreshToken, string newTokenHash, DateTime expiresAt,string ipaddress)
    {
        var newRefreshToken = Create(refreshToken.ApplicationUserId, newTokenHash, ipaddress, expiresAt);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipaddress;
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        return newRefreshToken;
    }
}