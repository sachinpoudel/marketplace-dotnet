using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Payments.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MarketPlace.Application.Features.Payments.Refund;

public class RefundPaymentCommandHandler(ICurrentUser currentUser, IPaymentRepository paymentRepository, IUnitOfWork unitOfWork) : IRequestHandler<RefundPaymentCommand, Result<RefundPaymentDto>>
{
    public async Task<Result<RefundPaymentDto>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
       var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<RefundPaymentDto>.Failure(UserError.UserNotAuthenticated());
        }

        var payment = await paymentRepository.GetPaymentByIdAsync(PaymentId.Create(request.PaymentId), cancellationToken);
        if (payment == null)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotFound());
        }
        if(payment.Status != Domain.Payments.Enums.PaymentStatus.Completed)
        {
            return Result<RefundPaymentDto>.Failure(PaymentError.PaymentNotPending());
        }
        if (payment.UserId != currentUserId)
        {
            return Result<RefundPaymentDto>.Failure(UserError.UserNotAuthorized());
        }

        payment.MarkAsRefunded();
        await unitOfWork.CommitAsync(cancellationToken);

        return Result<RefundPaymentDto>.Success(new RefundPaymentDto(payment.Id.Value, payment.Amount));
    }
}
