using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Vendors.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
     internal protected DbSet<Product> Products { get; set; } = null!;
     internal protected DbSet<Vendor> Vendors { get; set; } = null!;
     internal protected DbSet<Category> Categories { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        base.OnModelCreating(modelBuilder);


        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}