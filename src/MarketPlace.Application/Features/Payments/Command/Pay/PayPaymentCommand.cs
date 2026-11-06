using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Pay;

public record class PayPaymentCommand(Guid OrderId, PaymentMethod PaymentMethod)
    : IRequest<Result<PayPaymentDto>>;
