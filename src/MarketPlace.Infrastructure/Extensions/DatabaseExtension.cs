using MarketPlace.Domain.Options;
using MarketPlace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.Extensions;

public static class DatabaseExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContextPool<ApplicationDbContext>((provider, options) =>
        {
            var connString = provider.GetRequiredService<IOptions<ConnStringOption>>().Value.ConnectionString;

            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ArgumentException("Connection string is not configured");
            }

            options.UseNpgsql(connString, npgsqloptions =>
                    {
                        npgsqloptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorCodesToAdd: null
                        );
                        npgsqloptions.CommandTimeout(60);
                    });
        });

        return services;
    }
}