using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MarketPlace.Infrastructure.Persistence.Configurations;


public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => CategoryId.Create(value))
            .ValueGeneratedNever();

        builder.Property(c => c.ParentCategoryId)
            .HasConversion(new ValueConverter<CategoryId?, Guid?>(
                parentId => parentId == null ? null : parentId.Value,
                value => value.HasValue ? CategoryId.Create(value.Value) : null))
            .IsRequired(false);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.ImageUrl).HasMaxLength(500);
        builder.Property(c => c.DisplayOrder).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        builder.Ignore(c => c.Children);
        builder.Ignore(c => c.ProductIds);

    }
}