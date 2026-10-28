using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Refund;

public record class RefundPaymentCommand
(
    Guid PaymentId

): IRequest<Result<RefundPaymentDto>>;