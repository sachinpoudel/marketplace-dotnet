using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.PaymentGateway;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Enums;
using MarketPlace.Application.Common.Interfaces.PaymentGateway.Models;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Payments.Command.ProcessPendingRefunds;

public class ProcessPendingRefundsCommandHandler(IPaymentRepository paymentRepository, IPaymentGateway paymentGateway, IUnitOfWork unitOfWork, ILogger<ProcessPendingRefundsCommandHandler> logger) : IRequestHandler<ProcessPendingRefundsCommand>
{
  public async Task Handle(
        ProcessPendingRefundsCommand request,
        CancellationToken cancellationToken
    )
    {
        var payments = await paymentRepository.GetPendingRefundsAsync(
            10,
            cancellationToken
        );

        foreach (var payment in payments)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(payment.ExternalReferenceId)
                    || string.IsNullOrWhiteSpace(payment.TransactionId))
                {
                    logger.LogWarning(
                        "Payment {PaymentId} has missing Khalti references.",
                        payment.Id.Value
                    );
                    continue;
                }

                var lookup = await paymentGateway.VerifyPaymentAsync(
                    new PaymentVerifyRequest(payment.ExternalReferenceId),
                    cancellationToken
                );

                if (!lookup.IsSuccess
                    || lookup.ExternalReferenceId != payment.ExternalReferenceId
                    || lookup.Amount != payment.Amount
                    || lookup.TransactionId != payment.TransactionId)
                {
                    logger.LogWarning(
                        "Khalti verification failed for payment {PaymentId}.",
                        payment.Id.Value
                    );
                    continue;
                }

                switch (lookup.PaymentStatus)
                {
                    case GatewayPaymentStatus.Refunded:
                        payment.MarkAsRefunded(null);
                        break;

                    case GatewayPaymentStatus.Completed:
                        var refund = await paymentGateway.RefundPaymentAsync(
                            new PaymentRefundRequest(
                                payment.TransactionId,
                                payment.Amount
                            ),
                            cancellationToken
                        );

                        if (!refund.IsSuccess)
                        {
                            logger.LogWarning(
                                "Khalti refund failed for payment {PaymentId}: {Error}",
                                payment.Id.Value,
                                refund.ErrorMessage
                            );
                            continue;
                        }

                        payment.MarkAsRefunded(refund.RefundTransactionId);
                        break;

                    default:
                        logger.LogWarning(
                            "Payment {PaymentId} is not refundable at Khalti. Status: {Status}",
                            payment.Id.Value,
                            lookup.PaymentStatus
                        );
                        continue;
                }

                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInformation(
                    "Refund completed for payment {PaymentId}.",
                    payment.Id.Value
                );
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error processing refund for payment {PaymentId}.",
                    payment.Id.Value
                );
            }
        }
    }
}