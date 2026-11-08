using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.Enums;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Create;

public class CreatePaymentCommandHandler(
    ICurrentUser currentUser,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IPaymentGateway paymentGateway,
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
        if (order.Status == OrderStatus.Cancelled)
        {
            return Result<PaymentDetailsDto>.Failure(OrderError.OrderCancelled());
        }
        if (order.Status == OrderStatus.Confirmed)
        {
            return Result<PaymentDetailsDto>.Failure(OrderError.OrderAlreadyPaid());
        }
        if (order.UserId != currentUserId)
        {
            return Result<PaymentDetailsDto>.Failure(UserError.UserNotAuthorized());
        }

        var existingPayment = await paymentRepository.GetPaymentByOrderIdAsync(
            order.Id,
            cancellationToken
        );

        if (existingPayment is not null)
        {
            if (existingPayment.Status == Domain.Payments.Enums.PaymentStatus.Completed)
            {
                return Result<PaymentDetailsDto>.Failure(PaymentError.PaymentAlreadyPaid());
            }
            if (
                existingPayment.Status == PaymentStatus.Pending
                && !string.IsNullOrEmpty(existingPayment.ExternalReferenceId)
            )
            {
                var check = await paymentGateway.VerifyPaymentAsync(
                    new(existingPayment.ExternalReferenceId),
                    cancellationToken
                );
                if (check?.IsSuccess == true)
                    return Result<PaymentDetailsDto>.Failure(PaymentError.PaymentAlreadyPaid());
            }
        }
        if(request.Method != PaymentMethod.Khalti)
        {
            return Result<PaymentDetailsDto>.Failure(PaymentError.PaymentMethodNotSupported());
        }
    
        var khaltiRequest = new PaymentInitiateRequest(
            order.Id.Value,
            order.TotalAmount,
            order.ShippingAddress.FullName,
            order.ShippingAddress.PhoneNumber
        );
        var gatewayResponse = await paymentGateway.InitiatePaymentAsync(
            khaltiRequest,
            cancellationToken
        );
  
        Payment paymentResult;
        if (existingPayment is not null)
        {
            existingPayment.ResetForRetry(
                gatewayResponse.ExternalReferenceId.ToString(),
                request.Method
            );
            paymentResult = existingPayment;
        }
        else
        {
            var payment = Payment.Create(
                order.Id,
                currentUserId,
                order.TotalAmount,
                request.Method
            );
            payment.SetExternalReferenceId(gatewayResponse.ExternalReferenceId.ToString());
            paymentResult = await paymentRepository.CreatePaymentAsync(payment, cancellationToken);
        }

        await unitOfWork.CommitAsync(cancellationToken);
        return Result<PaymentDetailsDto>.Success(
            new PaymentDetailsDto(
                paymentResult.Id.Value,
                paymentResult.OrderId.Value,
                paymentResult.UserId,
                paymentResult.Amount,
                paymentResult.Status,
                paymentResult.Method,
                gatewayResponse.PaymentUrl,
                paymentResult.CreatedAt
            )
        );
    }
}
