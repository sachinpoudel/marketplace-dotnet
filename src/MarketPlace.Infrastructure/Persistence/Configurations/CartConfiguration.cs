using System;
using MarketPlace.Domain.Carts;
using MarketPlace.Domain.Carts.Entities;
using MarketPlace.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => CartId.Create(value))
            .ValueGeneratedNever();

builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);

        // builder.Property(c => c.UserId).IsRequired();
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
