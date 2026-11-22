using System;
using Hangfire;
using Hangfire.PostgreSql;
using MarketPlace.Application.Common.Interfaces.BackgroundJobs;
using MarketPlace.Infrastructure.BackgroundJobs;
using MarketPlace.Infrastructure.BackgroundJobs.Hangfire;
using MarketPlace.Infrastructure.ExternalServices.Email;
using MarketPlace.Infrastructure.Persistence.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.Extensions;

public static class HangfireExtension
{
 public static IServiceCollection AddHangfire(this IServiceCollection services)
    {
        services.AddHangfire((provider,config) =>
        {
            var connString = provider.GetRequiredService<IOptions<ConnectionSettings>>().Value.ConnectionString;
            var hangfireSettings = provider.GetRequiredService<IOptions<EmailSettings>>().Value;

               config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                {
                    if (string.IsNullOrWhiteSpace(connString))
                    {
                        throw new InvalidOperationException("Postgres connection string is not configured.");
                    }

                    options.UseNpgsqlConnection(connString);
                });


            services.AddHangfireServer(options =>
            {
                options.WorkerCount = hangfireSettings?.WorkerCount ?? 1;
            });
            services.AddScoped<IBackgroundJobService, HangfireJobService>();
        });
        return services;
    }
}
