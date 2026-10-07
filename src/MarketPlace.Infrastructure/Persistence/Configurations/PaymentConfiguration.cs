using System;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments;
using MarketPlace.Domain.Payments.ValueObjects;
using MarketPlace.Infrastructure.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPlace.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion(id => id.Value, value => PaymentId.Create(value));

        builder
            .Property(p => p.OrderId)
            .HasConversion(orderId => orderId.Value, value => OrderId.Create(value));

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Amount).IsRequired();

        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.Method).IsRequired();
        builder.Property(p => p.TransactionId).IsRequired(false);
        builder.Property(p => p.PaidAt).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
    }
}
