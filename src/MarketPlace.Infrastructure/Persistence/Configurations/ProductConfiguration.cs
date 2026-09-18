using System.Text.Json;
using System.Text.Json.Nodes;
using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.ValueObjects;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;


public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        //conversion for ProductId value object
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => ProductId.Create(value))
            .ValueGeneratedNever();






        builder.OwnsMany(p => p.Tags, tagBuilder =>
                {
                    tagBuilder.ToJson();
                    tagBuilder.Property(t => t.Name).HasMaxLength(50);
                    tagBuilder.Property(t => t.Slug).HasMaxLength(50);
                });




        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.Sku).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Status).IsRequired();

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();


        builder.Property(p => p.Images).
        HasField("_images")
        .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                images => JsonSerializer.Serialize(images.Select(i => i.Url), (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null)!
                    .Select(url => Img.Create(url))
                    .ToList())
            .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<Img>>(
                (a, b) => a!.Select(i => i.Url).SequenceEqual(b!.Select(i => i.Url)),
                c => c.Aggregate(0, (hash, i) => HashCode.Combine(hash, i.Url.GetHashCode())),
                c => c.ToList()));

        builder.Property(p => p.Images).IsRequired();


        builder.Property(p => p.VendorId)
            .HasConversion(vendorId => vendorId.Value, value => VendorId.Create(value))
            .IsRequired();

        builder.HasIndex(p => p.VendorId);

        builder.Property(p => p.CategoryIds).
        HasField("_categoryIds").UsePropertyAccessMode(PropertyAccessMode.Field)
       .HasConversion(
           categoryIds => JsonSerializer.Serialize(categoryIds.Select(c => c.Value), (JsonSerializerOptions?)null),
           json => JsonSerializer.Deserialize<List<Guid>>(json, (JsonSerializerOptions?)null)!
               .Select(guid => CategoryId.Create(guid))
               .ToList())
       .Metadata.SetValueComparer(new ValueComparer<IReadOnlyCollection<CategoryId>>(
           (a, b) => a!.SequenceEqual(b!),
           c => c.Aggregate(0, (hash, id) => HashCode.Combine(hash, id.Value.GetHashCode())),
           c => c.ToList()))
       ;


        builder.Ignore(p => p.CategoryIds);







    }
}
