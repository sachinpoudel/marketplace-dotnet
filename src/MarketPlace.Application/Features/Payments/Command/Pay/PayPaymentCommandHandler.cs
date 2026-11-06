using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Pay;

public class PayPaymentCommandHandler(
    ICurrentUser currentUser,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ICartRepository cartRepository,
    IOrderRepository orderRepository
) : IRequestHandler<PayPaymentCommand, Result<PayPaymentDto>>
{
    public async Task<Result<PayPaymentDto>> Handle(
        PayPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<PayPaymentDto>.Failure(UserError.UserNotAuthenticated());
        }

        var payment = await paymentRepository.GetPaymentByOrderIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );
        if (payment == null)
        {
            return Result<PayPaymentDto>.Failure(PaymentError.PaymentNotFound());
        }
        if(payment.Status != Domain.Payments.Enums.PaymentStatus.Pending)
        {
            return Result<PayPaymentDto>.Failure(PaymentError.PaymentNotPending());
        }
        if (payment.UserId != currentUserId)
        {
            return Result<PayPaymentDto>.Failure(UserError.UserNotAuthorized());
        }
        var order = await orderRepository.GetOrderByIdAsync(
            OrderId.Create(request.OrderId),
            cancellationToken
        );
        if (order == null)
        {
            return Result<PayPaymentDto>.Failure(OrderError.OrderNotFound());
        }
        if(order.Status == Domain.Orders.Enums.OrderStatus.Cancelled )
        {
            return Result<PayPaymentDto>.Failure(OrderError.OrderCancelled());
        }



await unitOfWork.BeginTransactionAsync(cancellationToken); // this line will start a transaction, so if any of the following operations fail, we can rollback the transaction and not commit any changes to the database.
payment.MarkAsPaid();

await unitOfWork.CommitAsync(cancellationToken);
        payment.MarkAsPaid();
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
