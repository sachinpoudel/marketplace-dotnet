using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;


public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => ProductId.Create(value))
            .ValueGeneratedNever();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.Sku).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Tags).HasMaxLength(200);
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.Ignore(p => p.CategoryIds);
        builder.Ignore(p => p.ReviewsId);
        builder.Ignore(p => p.Images);

        builder.Property(p => p.VendorId)
            .HasConversion(vendorId => vendorId.Value, value => VendorId.Create(value))
            .IsRequired();




    }
}