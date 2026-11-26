using System.Xml.Serialization;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.BackgroundJobs;
using MarketPlace.Application.Common.Interfaces.Email;
using MarketPlace.Application.Common.Interfaces.ImageStorage;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.Query;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Orders.Jobs;
using MarketPlace.Infrastructure.BackgroundJobs;
using MarketPlace.Infrastructure.BackgroundJobs.Email;
using MarketPlace.Infrastructure.BackgroundJobs.Hangfire;
using MarketPlace.Infrastructure.Extensions;
using MarketPlace.Infrastructure.ExternalServices.Cloudinary;
using MarketPlace.Infrastructure.ExternalServices.Email;
using MarketPlace.Infrastructure.ExternalServices.Payments.khalti;
using MarketPlace.Infrastructure.Identity.Models;
using MarketPlace.Infrastructure.Identity.Services;
using MarketPlace.Infrastructure.Persistence;
using MarketPlace.Infrastructure.Persistence.Queries;
using MarketPlace.Infrastructure.Persistence.Repositories;
using MarketPlace.Infrastructure.Persistence.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;

namespace MarketPlace.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints()
            .AddDefaultTokenProviders();

        services.AddJwt();
        services.AddHangfire();

        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddScoped<IHttpContextProvider, HttpContextProvider>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IProductQuery, ProductQuery>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IAuthService, IdentityAuthService>();
        services.AddScoped<JwtTokenService>();
        services.AddScoped<IdentitySeeder>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IOrderConfirmationEmailJob, OrderConfirmationEmailJob>();
        services.AddScoped<IBackgroundJobService, HangfireJobService>();
        services.AddScoped<IImageStorage, CloudinaryStorageService>();

        services
            .AddHttpClient<IPaymentGateway, KhaltiGateway>(
                (serviceProvider, client) =>
                {
                    var settings = serviceProvider
                        .GetRequiredService<IOptions<KhaltiSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(settings.BaseUrl);

                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                            "application/json"
                        )
                    );
                }
            )
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.DisableForUnsafeHttpMethods();
                ConfigureKhaltiResilience(options);
            });

        services
            .AddHttpClient(
                "KhaltiLookup",
                (serviceProvider, client) =>
                {
                    var settings = serviceProvider
                        .GetRequiredService<IOptions<KhaltiSettings>>()
                        .Value;

                    client.BaseAddress = new Uri(settings.BaseUrl);

                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                            "application/json"
                        )
                    );
                }
            )
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 2;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.Retry.BackoffType = DelayBackoffType.Exponential;
                options.Retry.UseJitter = true;

                ConfigureKhaltiResilience(options);
            });

        services.AddHostedService<ExpiredReservationWorker>();
        services.AddHostedService<PendingRefundWorker>();

        return services;
    }

    private static void ConfigureKhaltiResilience(HttpStandardResilienceOptions options)
    {
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(20);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(8);

        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
    }
}
// Lookup gets at most 2 retries after the initial attempt.



//Retry delay uses exponential backoff with jitter.



//Each attempt has an 8-second timeout.


//The entire operation, including retries, has a 20-second timeout//.



// The circuit breaker opens when at least 5 requests have been observed in its 30-second sampling window and at least 50% fail. It then stays open for 10 seconds.