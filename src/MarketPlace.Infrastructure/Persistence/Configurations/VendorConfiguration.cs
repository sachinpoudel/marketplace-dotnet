using MarketPlace.Domain.Vendors.Entities;
using MarketPlace.Domain.Vendors.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;


public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id)
            .HasConversion(id => id.Value, value => VendorId.Create(value))
            .ValueGeneratedNever();

        builder.Property(v => v.LegalName).IsRequired().HasMaxLength(100);
        builder.Property(v => v.TradeName).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Description).HasMaxLength(1000);
        builder.Property(v => v.ProfileUrl).HasMaxLength(500);
        builder.Property(v => v.Status).IsRequired();
        builder.Property(v => v.BusinessAddress).IsRequired().HasMaxLength(250);
        builder.Property(v => v.ContactEmail).IsRequired().HasMaxLength(200);
        builder.Property(v => v.CreatedAt).IsRequired();
        builder.Property(v => v.UpdatedAt).IsRequired();

    }
}