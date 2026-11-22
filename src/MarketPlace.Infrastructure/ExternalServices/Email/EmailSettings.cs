using System;

namespace MarketPlace.Infrastructure.ExternalServices.Email;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public int WorkerCount { get; set; } = 1;
}
