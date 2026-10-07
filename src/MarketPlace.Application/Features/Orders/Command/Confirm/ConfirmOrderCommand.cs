using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Confirm;

public record class ConfirmOrderCommand
(
    Guid OrderId
): IRequest<Result<Unit>>;