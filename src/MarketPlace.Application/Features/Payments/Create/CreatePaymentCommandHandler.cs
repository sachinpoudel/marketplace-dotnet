using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.Enums;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Create;

public class CreatePaymentCommandHandler(
    ICurrentUser currentUser,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreatePaymentCommand, Result<PaymentDetailsDto>>
{
    public async Task<Result<PaymentDetailsDto>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<PaymentDetailsDto>.Failure(UserError.UserNotAuthenticated());
        }
        var order = await orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );
        if (order == null)
        {
            return Result<PaymentDetailsDto>.Failure(OrderError.OrderNotFound());
        }
        if(order.Status  == OrderStatus.Cancelled)
        {
            return Result<PaymentDetailsDto>.Failure(OrderError.OrderCancelled());
        }
        if (order.UserId != currentUserId)
        {
            return Result<PaymentDetailsDto>.Failure(UserError.UserNotAuthorized());
        }
        var payment = Payment.Create(
            order.Id,
            currentUserId,
            order.TotalAmount,
            request.Method,
   
            request.PaidAt
        );

        var paymentCreated = await paymentRepository.CreatePaymentAsync(payment, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<PaymentDetailsDto>.Success(
            new PaymentDetailsDto(
                paymentCreated.Id.Value,
                paymentCreated.OrderId.Value,
                paymentCreated.UserId,
                paymentCreated.Amount,
                paymentCreated.Status,
                paymentCreated.Method,
           
                paymentCreated.PaidAt,
                paymentCreated.CreatedAt
            )
        );
    }
}
