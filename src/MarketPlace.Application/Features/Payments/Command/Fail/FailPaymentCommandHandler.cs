using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MarketPlace.Domain.Payments.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Fail;

public class FailPaymentCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork) : IRequestHandler<FailPaymentCommand, Result>
{
    public async Task<Result> Handle(FailPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetPaymentByOrderIdAsync(OrderId.Create(request.OrderId), cancellationToken);

        if(payment is null)
        {
            return Result.Failure(PaymentError.PaymentNotFound());
        }
        if(payment.Status == Domain.Payments.Enums.PaymentStatus.Completed)
        {
            return Result.Success();
        }
        payment.MarkAsFailed();
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
