using System.Text;
using System.Text.Json;
using MarketPlace.Infrastructure.Identity.Models;
using MarketPlace.Infrastructure.Persistence;
using MarketPlace.Infrastructure.Persistence.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MarketPlace.Infrastructure.Extensions;


public static class JwtExtension
{
    public static IServiceCollection AddJwt(this IServiceCollection services)
    {

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddApiEndpoints()
             .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
                       {
                           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                           options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                       })
                       .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });


        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                 .Configure<IOptions<JwtOptions>>((options, jwtOptions) =>
                        {
                            var jwtSettings = jwtOptions.Value;

                            if (string.IsNullOrEmpty(jwtSettings.SigningKey) || string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience))
                            {
                                throw new InvalidOperationException("JWT configuration is missing or invalid.");
                            }

                            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));

                            options.RequireHttpsMetadata = false;
                            //options.SaveToken = true;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = jwtSettings.Issuer,
                                ValidAudience = jwtSettings.Audience,
                                IssuerSigningKey = secretKey,
                                ClockSkew = TimeSpan.Zero
                            };
                            options.Events = new JwtBearerEvents
                            {
                                OnChallenge = context =>
                                {
                                    context.HandleResponse();
                                    context.Response.StatusCode = 401;
                                    context.Response.ContentType = "application/json";
                                    var result = JsonSerializer.Serialize(new
                                    {
                                        message = "You are not authorized to access this resource. Please authenticate."
                                    });
                                    return context.Response.WriteAsync(result);
                                }
                            };
                        });




        return services;
    }
}
