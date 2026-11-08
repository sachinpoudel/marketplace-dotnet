using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Pay;

public class PayPaymentCommandHandler(
    ICurrentUser currentUser,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ICartRepository cartRepository,
    IPaymentGateway paymentGateway,
    IOrderRepository orderRepository
) : IRequestHandler<PayPaymentCommand, Result<PayPaymentDto>>
{
   public async Task<Result<PayPaymentDto>> Handle(
    PayPaymentCommand request,
    CancellationToken cancellationToken
)
{
    var payment = await paymentRepository.GetPaymentByOrderIdAsync(
        OrderId.Create(request.OrderId),
        cancellationToken
    );
    if (payment is null)
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentNotFound());
    }

    if (payment.Status == PaymentStatus.Completed)
    {
        return Result<PayPaymentDto>.Success(
            new PayPaymentDto(
                payment.Id.Value,
                payment.OrderId.Value,
                payment.UserId,
                payment.Amount,
                payment.Status,
                payment.Method,
                payment.PaidAt,
                payment.TransactionId
            )
        );
    }

    if (payment.Status != PaymentStatus.Pending)
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentNotPending());
    }

    if (string.IsNullOrWhiteSpace(payment.ExternalReferenceId))
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentGatewayError());
    }

    var order = await orderRepository.GetOrderByIdAsync(
        OrderId.Create(request.OrderId),
        cancellationToken
    );
    if (order is null)
    {
        return Result<PayPaymentDto>.Failure(OrderError.OrderNotFound());
    }

    if (order.Status == Domain.Orders.Enums.OrderStatus.Cancelled)
    {
        return Result<PayPaymentDto>.Failure(OrderError.OrderCancelled());
    }

    var verifyRequest = new PaymentVerifyRequest(payment.ExternalReferenceId);
    var verificationResult = await paymentGateway.VerifyPaymentAsync(
        verifyRequest,
        cancellationToken
    );

    if (verificationResult is null || !verificationResult.IsSuccess)
    {
        Console.WriteLine(
            $"Payment verification failed for ExternalReferenceId: {payment.ExternalReferenceId}"
        );
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentGatewayError());
    }

    Console.WriteLine(
        $"Payment verification result for ExternalReferenceId: {verificationResult.ExternalReferenceId} - IsSuccess: {verificationResult.IsSuccess}, Amount: {verificationResult.Amount}, TransactionId: {verificationResult.TransactionId}"
    );

    payment.MarkAsPaid(verificationResult.TransactionId ?? string.Empty);
    await unitOfWork.CommitAsync(cancellationToken);

    return Result<PayPaymentDto>.Success(
        new PayPaymentDto(
            payment.Id.Value,
            payment.OrderId.Value,
            payment.UserId,
            payment.Amount,
            payment.Status,
            payment.Method,
            payment.PaidAt,
            payment.TransactionId
        )
    );
}

}
