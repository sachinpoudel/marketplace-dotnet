using MarketPlace.Domain.Options;

namespace MarketPlace.Api.Extensions;


public static class WebExtension
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
builder.Services.Configure<ConnStringOption>(builder.Configuration.GetSection("ConnectionStrings"));
        builder.Services.AddControllers();
    }
}