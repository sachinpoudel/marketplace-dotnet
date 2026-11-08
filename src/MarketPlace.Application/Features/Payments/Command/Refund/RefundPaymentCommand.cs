using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Refund;

public record class RefundPaymentCommand
(
    Guid OrderId,
    Guid PaymentId,
    bool IsManual = false

): IRequest<Result<RefundPaymentDto>>;