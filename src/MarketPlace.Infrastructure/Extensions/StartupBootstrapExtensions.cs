using MarketPlace.Infrastructure.Identity.Services;
using MarketPlace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MarketPlace.Infrastructure.Extensions;

public static class StartupBootstrapExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync(); // this call will apply any pending migrations for the context to the database. Will create the database if it does not already exist.
    }

    public static async Task SeedIdentityRolesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

        await seeder.SeedAsync(CancellationToken.None);
    }
}