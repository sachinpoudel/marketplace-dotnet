using Microsoft.AspNetCore.Identity;

namespace MarketPlace.Infrastructure.Identity.Services;

public sealed class IdentitySeeder
{
    private static readonly string[] DefaultRoles = ["Customer", "Vendor", "Admin"];

    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public IdentitySeeder(RoleManager<IdentityRole<Guid>> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        foreach (var roleName in DefaultRoles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
}