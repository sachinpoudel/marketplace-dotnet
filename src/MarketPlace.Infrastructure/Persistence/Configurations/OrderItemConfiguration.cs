using System;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Domain.Orders.Entities;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{


    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id).HasConversion(oi => oi.Value, value => OrderItemId.Create(value)).ValueGeneratedNever();


        builder.Property(oi => oi.ProductId).HasConversion(oi => oi.Value, value => ProductId.Create(value)).IsRequired();


        builder.Property(oi => oi.VendorId).HasConversion(oi => oi.Value, value => VendorId.Create(value)).IsRequired();

        builder.Property(static oi => oi.OrderId).HasConversion(oi => oi.Value, value => OrderId.Create(value)).IsRequired();

        builder.Property(oi => oi.Quantity).IsRequired();
        builder.Property(oi => oi.ProductName).IsRequired();
      builder.Ignore(static oi => oi.SubTotal);
        builder.Property(oi => oi.Price).IsRequired();

    }
}
