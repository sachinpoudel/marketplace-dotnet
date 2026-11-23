namespace MarketPlace.Infrastructure.Persistence.Options;


public class ConnectionSettings
{
    public const string SectionName = "ConnectionSettings";
        public string ConnectionString { get; set; } = string.Empty;
}