using System;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Email;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Orders.Jobs;
using MarketPlace.Domain.Orders.ValueObjects;

namespace MarketPlace.Infrastructure.BackgroundJobs.Email;

public class OrderConfirmationEmailJob(IOrderRepository orderRepository, ICurrentUser currentUser, IEmailService emailService) : IOrderConfirmationEmailJob
{
    public async Task ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
       var order = await orderRepository.GetOrderByIdAsync(OrderId.Create(orderId), cancellationToken);
       if (order is null) return;

   


        var subject = $"Order Confirmation - Order #{order.Id.Value}";
        var body = $"Dear {currentUser.UserName},\n\nThank you for your order! Your order with ID {order.Id.Value} has been successfully placed.\n\nOrder Details:\n";
        foreach (var item in order.Items)
        {
            body += $"- Product ID: {item.ProductId}, Quantity: {item.Quantity}\n";
        }
        body += $"\nTotal Amount: {order.TotalAmount}\n\nWe appreciate your business and hope you enjoy your purchase!\n\nBest regards,\nMarketPlace Team";
        var userEmail =   currentUser.Email;
        await emailService.SendEmailAsync(userEmail, subject, body, cancellationToken);
    }
}
