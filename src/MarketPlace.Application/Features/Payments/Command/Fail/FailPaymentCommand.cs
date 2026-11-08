using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.Fail;

public record class FailPaymentCommand (Guid OrderId): IRequest<Result>;