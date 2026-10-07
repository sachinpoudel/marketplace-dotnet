using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Pay;

public record class PayPaymentCommand
(
    Guid OrderId,
  PaymentMethod PaymentMethod,
  DateTime PaidAt
): IRequest<Result<PayPaymentDto>>;
