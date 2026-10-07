using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Create;

public record class CreatePaymentCommand
(
  Guid Id,
  Guid OrderId,

    PaymentMethod Method,

  DateTime PaidAt
) : IRequest<Result<PaymentDetailsDto>>;
