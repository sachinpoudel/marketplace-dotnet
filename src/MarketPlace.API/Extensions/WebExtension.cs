using MarketPlace.Infrastructure.Persistence.Options;
using PortfolioApp.WebApi.Handlers;
using Serilog;

namespace MarketPlace.Api.Extensions;


public static class WebExtension
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
      
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
        builder.Services.AddCors(options =>
     {
         options.AddPolicy("myPolicyName",
             policy =>
             {
                 policy.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
             });
         options.AddPolicy("AllowOnlySomeOrigins",
             policy => { policy.WithOrigins("http://localhost:3000/"); });
     });


        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        // Always include useful metadata
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
        ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
    };
});
        builder.Services.Configure<ConnStringOption>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();
    }
}