using MarketPlace.Application.Features.Payments.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Payments.Enums;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Create;

public record class CreatePaymentCommand(Guid OrderId, PaymentMethod Method)
    : IRequest<Result<PaymentDetailsDto>>;
