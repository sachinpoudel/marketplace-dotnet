using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Enums;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MarketPlace.Domain.Payments.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MarketPlace.Application.Features.Payments.Command.Refund;

public class RefundPaymentCommandHandler(
    ICurrentUser currentUser,
    IPaymentGateway paymentGateway,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentDto>>
{
    public async Task<Result<RefundPaymentDto>> Handle(
        RefundPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<RefundPaymentDto>.Failure(UserError.UserNotAuthenticated());
        }

        var payment = await paymentRepository.GetPaymentByIdAsync(
            PaymentId.Create(request.PaymentId),
            cancellationToken
        );
        if (payment == null)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotFound());
        }
        if (payment.OrderId.Value != request.OrderId)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotFound());
        }
        if (payment.UserId != currentUserId)
        {
            return Result<RefundPaymentDto>.Failure(UserError.UserNotAuthorized());
        }
        if (payment.Status == PaymentStatus.Refunded) // this check is to ensure that the payment has not already been refunded ie idempotency check
            return Result<RefundPaymentDto>.Success(
                new RefundPaymentDto(payment.Id.Value, payment.Amount)
            );

        if (payment.Status != PaymentStatus.Completed)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotPending());
        }
        var order = await orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );
        if (order == null)
        {
            return Result<RefundPaymentDto>.Failure(OrderError.OrderNotFound());
        }
        if (order.UserId != currentUserId)
        {
            return Result<RefundPaymentDto>.Failure(UserError.UserNotAuthorized());
        }

        var lookup = await paymentGateway.VerifyPaymentAsync(
            new(payment.ExternalReferenceId!),
            cancellationToken
        );

        if (lookup is null || !lookup.IsSuccess)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentGatewayError());
        }

        if (
            lookup.ExternalReferenceId != payment.ExternalReferenceId
            || lookup.Amount != payment.Amount
            || lookup.TransactionId != payment.TransactionId
        )
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentGatewayError());
        }

        switch (lookup.PaymentStatus)
        {
            case GatewayPaymentStatus.Refunded:
                // Khalti already refunded it. Reconcile local state.
                payment.MarkAsRefunded(null);

                await unitOfWork.CommitAsync(cancellationToken);

                return Result<RefundPaymentDto>.Success(
                    new RefundPaymentDto(payment.Id.Value, payment.Amount)
                );

            case GatewayPaymentStatus.Completed:
                break;

            default:
                return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotRefunded());
        }

        var khaltiRefundRequest = new PaymentRefundRequest(payment.TransactionId, payment.Amount);
        var khaltiRefundResponse = await paymentGateway.RefundPaymentAsync(
            khaltiRefundRequest,
            cancellationToken
        );
        if (khaltiRefundResponse == null || !khaltiRefundResponse.IsSuccess)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotRefunded());
        }
        var result = payment.MarkAsRefunded(khaltiRefundResponse.RefundTransactionId);
        if (result.IsFailure)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotRefunded());
        }
        await unitOfWork.CommitAsync(cancellationToken);

        return Result<RefundPaymentDto>.Success(
            new RefundPaymentDto(payment.Id.Value, payment.Amount)
        );
    }
}
