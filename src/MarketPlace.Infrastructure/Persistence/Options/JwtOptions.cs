namespace MarketPlace.Infrastructure.Persistence.Options;


public class JwtOptions
{

    public const string SectionName = "JwtOptions";
    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } 
    public int RefreshTokenDays { get; set; } 
}