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
        builder
            .Property(p => p.Id)
            .HasConversion(id => id.Value, value => ProductId.Create(value))
            .ValueGeneratedNever();

        builder.OwnsMany(
            p => p.Tags,
            tagBuilder =>
            {
                tagBuilder.ToJson();
                tagBuilder.Property(t => t.Name).HasMaxLength(50);
                tagBuilder.Property(t => t.Slug).HasMaxLength(50);
            }
//             OwnsMany() means:
// - A Product can own multiple Tags.
// - Tags belong to the Product's lifecycle.
// - Tags aren't independently managed aggregate roots.
        );

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.Sku).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Status).IsRequired();
        builder.HasQueryFilter(p => !p.IsDeleted);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        builder.Property<uint>("xmin").IsRowVersion(); //this line will create a shadow property called "xmin" that will be used for optimistic concurrency control. The IsRowVersion() method tells EF Core to treat this property as a row version, which means that it will automatically be updated by the database whenever the row is updated. This allows EF Core to detect when a row has been modified by another user and throw a DbUpdateConcurrencyException if a conflict occurs.
        builder
            .Property(p => p.Images) // this line tells EF Core that we want to configure the Images property of the Product entity.
            .HasField("_images")// this line tells EF Core that the Images property is backed by a private field called _images. This means that EF Core will use the _images field to store the value of the Images property in the database, rather than using a separate column for each image.
            .UsePropertyAccessMode(PropertyAccessMode.Field).HasColumnType("jsonb") // this line tells EF Core to use the field access mode when reading and writing the Images property. This means that EF Core will bypass the property getter and setter and access the _images field directly. This is necessary because the Images property is read-only and does not have a public setter.
            .HasConversion( // converts the Images property to and from a JSON string for storage in the database. The HasConversion method takes two parameters: a function that converts the property value to a database value, and a function that converts the database value back to a property value.
                images =>
                    JsonSerializer.Serialize(
                        images.Select(i => i.Url),
                        (JsonSerializerOptions?)null
                    ),
                json =>
                    JsonSerializer
                        .Deserialize<List<string>>(json, (JsonSerializerOptions?)null)!
                        .Select(url => Img.Create(url))
                        .ToList()
            )
            .Metadata.SetValueComparer( // this line tells EF Core to use a custom value comparer for the Images property. The ValueComparer class is used to compare two values of a given type for equality. In this case, we are comparing two IReadOnlyCollection<Img> values for equality by comparing their Url properties. The ValueComparer class takes three parameters: a function that compares two values for equality, a function that computes a hash code for a value, and a function that creates a copy of a value.
                new ValueComparer<IReadOnlyCollection<Img>>(
                    (a, b) => a!.Select(i => i.Url).SequenceEqual(b!.Select(i => i.Url)),
                    c => c.Aggregate(0, (hash, i) => HashCode.Combine(hash, i.Url.GetHashCode())),
                    c => c.ToList()
                )
            );

        builder.Property(p => p.Images).IsRequired();

        builder
            .Property(p => p.VendorId)
            .HasConversion(vendorId => vendorId.Value, value => VendorId.Create(value))
            .IsRequired();

        builder.HasIndex(p => p.VendorId);

        builder
            .Property(p => p.CategoryIds)
            .HasField("_categoryIds")
            .UsePropertyAccessMode(PropertyAccessMode.Field).HasColumnType("jsonb")
            .HasConversion(
                categoryIds =>
                    JsonSerializer.Serialize(
                        categoryIds.Select(c => c.Value),
                        (JsonSerializerOptions?)null
                    ),
                json =>
                    JsonSerializer
                        .Deserialize<List<Guid>>(json, (JsonSerializerOptions?)null)!
                        .Select(guid => CategoryId.Create(guid))
                        .ToList()
            )
            .Metadata.SetValueComparer(
                new ValueComparer<IReadOnlyCollection<CategoryId>>(
                    (a, b) => a!.SequenceEqual(b!),
                    c =>
                        c.Aggregate(
                            0,
                            (hash, id) => HashCode.Combine(hash, id.Value.GetHashCode())
                        ),
                    c => c.ToList()
                )
            );

        // builder.Ignore(p => p.CategoryIds); ignore means This means EF Core:
// - Does not create a database column for that property through this entity mapping.
// - Does not load its value from the database.
// - Does not save its value to the database.
// - Does not track changes to it as a mapped proper
     }
}
