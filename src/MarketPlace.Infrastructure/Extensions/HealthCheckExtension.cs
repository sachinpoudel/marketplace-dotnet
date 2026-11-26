using System;
using MarketPlace.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MarketPlace.Infrastructure.Extensions;

public static class HealthCheckExtension
{
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>(
            name: "postgresql", failureStatus: HealthStatus.Unhealthy, tags: ["ready"]);

        return services;
    }
}