using System;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments;
using MarketPlace.Domain.Payments.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;

public interface IPaymentRepository
{
   Task<Payment>  CreatePaymentAsync(Payment payment, CancellationToken cancellationToken = default);
   Task<Payment?> GetPaymentByIdAsync(PaymentId paymentId, CancellationToken cancellationToken = default);
   Task<Payment?> GetPaymentByOrderIdAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
