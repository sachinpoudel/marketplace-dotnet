using System;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace MarketPlace.API.Extensions;

public static class ObervabilityExtension
{
    public static IServiceCollection AddObervability(this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("MarketPlace.API"))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });
        return services;
    }
}
