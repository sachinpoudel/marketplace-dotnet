using System;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class OrderConfigurations : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, value => OrderId.Create(value))
            .ValueGeneratedNever();

        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.TotalAmount).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.ShippingCost).IsRequired();
        builder.Property(o => o.SubTotal).IsRequired();
        builder.Property(o => o.DiscountAmount).IsRequired();

        builder.OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(s => s.FullName).HasColumnName("FullName").IsRequired();
            sa.Property(s => s.City).HasColumnName("City").IsRequired();
            sa.Property(s => s.State).HasColumnName("State").IsRequired();
            sa.Property(s => s.AddressLine1).HasColumnName("AddressLine1").IsRequired();
            sa.Property(s => s.AddressLine2).HasColumnName("AddressLine2").IsRequired();
            sa.Property(s => s.PhoneNumber).HasColumnName("PhoneNumber").IsRequired();
        });

        builder.HasMany(o => o.Items)
               .WithOne()
               .HasForeignKey(i => i.OrderId)
               .OnDelete(DeleteBehavior.Cascade);


               builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(o => o.UserId).OnDelete(DeleteBehavior.Cascade);

               
    }
}
