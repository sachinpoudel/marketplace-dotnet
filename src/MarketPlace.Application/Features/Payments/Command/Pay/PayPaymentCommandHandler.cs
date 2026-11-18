using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Pay;

public class PayPaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
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
      
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentGatewayError());
    }
    if(verificationResult.Amount != payment.Amount)
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentAmountMismatch());
    }
    if (!string.Equals( 
        request.Pidx,
        payment.ExternalReferenceId,
        StringComparison.Ordinal))
{
    return Result<PayPaymentDto>.Failure(
        PaymentError.PaymentGatewayError()
    );
}
    if(verificationResult.ExternalReferenceId != payment.ExternalReferenceId) 
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentGatewayError());
    }
    if(string.IsNullOrWhiteSpace(verificationResult.TransactionId))
    {
        return Result<PayPaymentDto>.Failure(PaymentError.PaymentGatewayError());
    }
   
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
