using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.API.Extensions;

public static class HealthCheckEndpointExtension
{
    public static WebApplication MapHealthCheckEndpoint(this WebApplication app)
    {
        app.MapHealthChecks(
            "/health/ready",
            new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains("ready"),
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(
                        new
                        {
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new
                            {
                                name = entry.Key,
                                status = entry.Value.Status.ToString(),
                                exception = entry.Value.Exception?.Message,
                                duration = entry.Value.Duration.ToString(),
                            }),
                        }
                    );
                    await context.Response.WriteAsync(result);
                },
            }
        );
        app.MapHealthChecks(
            "/health/live",
            new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
            {
                Predicate = (_) => false,
            }
        );
        return app;
    }
}
