using System;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Carts.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => CartItemId.Create(value))

            .ValueGeneratedNever();
            builder.Property(c => c.CartId).HasConversion(cartId => cartId.Value, value => CartId.Create(value)).IsRequired();

        builder.Property(c => c.CartId).IsRequired();
        builder.Property(c => c.ProductId).IsRequired();
        builder.Property(c => c.Quantity).IsRequired();
    }
}
