using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments;
using MarketPlace.Domain.Payments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public class PaymentRepository(ApplicationDbContext context) : IPaymentRepository
{
    public async Task<Payment> CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        var result = await context.Payments.AddAsync(payment, cancellationToken);
        return result.Entity;
    }

    public async Task<Payment?> GetPaymentByIdAsync(PaymentId paymentId, CancellationToken cancellationToken = default)
    {
        return await context.Payments.FindAsync(paymentId, cancellationToken);
    }

    public async Task<Payment?> GetPaymentByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        var payment = await context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
        return payment;
    }
}
