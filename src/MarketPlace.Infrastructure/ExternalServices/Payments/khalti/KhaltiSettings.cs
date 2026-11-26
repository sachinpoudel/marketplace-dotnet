using System;

namespace MarketPlace.Infrastructure.ExternalServices.Payments.khalti;

public class KhaltiSettings 
{
    public const string SectionName = "KhaltiSettings";
  public string ReturnUrl { get; set; } = string.Empty;
  public string WebsiteUrl { get; set; } = string.Empty;
  public string SecretKey { get; set; } = string.Empty;
  public string BaseUrl {get;set;} = string.Empty;
  public int TimeoutInSeconds { get; set; } = 30; // Default timeout
}
