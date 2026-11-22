
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
            return Result<RefundPaymentDto>.Failure(
                UserError.UserNotAuthenticated());

        var payment = await paymentRepository.GetPaymentByIdAsync(
            PaymentId.Create(request.PaymentId),
            cancellationToken
        );

        if (payment == null)
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentNotFound());

        if (payment.OrderId.Value != request.OrderId)
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentNotFound());

        if (payment.UserId != currentUserId)
            return Result<RefundPaymentDto>.Failure(
                UserError.UserNotAuthorized());

        if (payment.Status == PaymentStatus.Refunded)
            return Result<RefundPaymentDto>.Success(
                new RefundPaymentDto(payment.Id.Value, payment.Amount));

        if (payment.Status != PaymentStatus.Completed &&
            payment.Status != PaymentStatus.RefundPending)
        {
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentNotPending());
        }

        var order = await orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );

        if (order == null)
            return Result<RefundPaymentDto>.Failure(
                OrderError.OrderNotFound());

        if (order.UserId != currentUserId)
            return Result<RefundPaymentDto>.Failure(
                UserError.UserNotAuthorized());

        if (payment.Status == PaymentStatus.Completed)
        {
            payment.MarkAsRefundPending();
            await unitOfWork.CommitAsync(cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(payment.ExternalReferenceId) ||
            string.IsNullOrWhiteSpace(payment.TransactionId))
        {
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentGatewayError());
        }

        var lookup = await paymentGateway.VerifyPaymentAsync(
            new(payment.ExternalReferenceId),
            cancellationToken
        );

        if (lookup is null || !lookup.IsSuccess)
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentGatewayError());

        if (lookup.ExternalReferenceId != payment.ExternalReferenceId ||
            lookup.Amount != payment.Amount ||
            lookup.TransactionId != payment.TransactionId)
        {
            return Result<RefundPaymentDto>.Failure(
                PaymentError.PaymentGatewayError());
        }

        switch (lookup.PaymentStatus)
        {
            case GatewayPaymentStatus.Refunded:
            {
                payment.MarkAsRefunded(null);
                break;
            }

            case GatewayPaymentStatus.Completed:
            {
                var refund = await paymentGateway.RefundPaymentAsync(
                    new PaymentRefundRequest(
                        payment.TransactionId,
                        payment.Amount
                    ),
                    cancellationToken
                );

                if (refund is null || !refund.IsSuccess)
                    return Result<RefundPaymentDto>.Failure(
                        PaymentError.PaymentNotRefunded());

                payment.MarkAsRefunded(refund.RefundTransactionId);
                break;
            }

            default:
                return Result<RefundPaymentDto>.Failure(
                    PaymentError.PaymentNotRefunded());
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return Result<RefundPaymentDto>.Success(
            new RefundPaymentDto(payment.Id.Value, payment.Amount)
        );
    }
}
