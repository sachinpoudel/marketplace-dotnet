using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Payments;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Reviews.Entities;
using MarketPlace.Domain.Vendors.Entities;
using MarketPlace.Infrastructure.Identity;
using MarketPlace.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace MarketPlace.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    protected internal DbSet<Product> Products { get; set; } = null!;
    protected internal DbSet<Vendor> Vendors { get; set; } = null!;
    protected internal DbSet<Category> Categories { get; set; } = null!;
    protected internal DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    protected internal DbSet<Review> Reviews { get; set; } = null!;
    protected internal DbSet<Cart> Carts { get; set; } = null!;
    protected internal DbSet<Order> Orders { get; set; } = null!;
    protected internal DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
