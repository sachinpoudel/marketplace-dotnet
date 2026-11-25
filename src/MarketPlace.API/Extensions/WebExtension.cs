using MarketPlace.API.Extensions;
using MarketPlace.API.Handlers;
using MarketPlace.Infrastructure.BackgroundJobs.Hangfire;
using MarketPlace.Infrastructure.Extensions;
using MarketPlace.Infrastructure.ExternalServices.Email;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti;
using MarketPlace.Infrastructure.Persistence.Options;
using Serilog;

namespace MarketPlace.Api.Extensions;

public static class WebExtension
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            }
        );
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "myPolicyName",
                policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
            );
            options.AddPolicy(
                "AllowOnlySomeOrigins",
                policy =>
                {
                    policy.WithOrigins("http://localhost:3000/");
                }
            );
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                // Always include useful metadata
                ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                ctx.ProblemDetails.Instance =
                    $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
            };
        });
        builder.Services.Configure<ConnectionSettings>(
            builder.Configuration.GetSection("ConnectionSettings")
        );
        builder.Services.Configure<JwtSettings>(
            builder.Configuration.GetSection("JwtSettings")
        );
        builder.Services.Configure<KhaltiSettings>(
            builder.Configuration.GetSection("KhaltiSettings")

        );
        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings")
        );
        builder.Services.Configure<HangfireSettings>(
            builder.Configuration.GetSection("HangfireSettings")
        );

        builder.Services.AddHealthCheckServices();
        builder.Services.AddObervability();
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
    }
}
