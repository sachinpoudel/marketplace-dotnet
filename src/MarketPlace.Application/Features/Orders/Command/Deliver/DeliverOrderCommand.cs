using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Deliver;

public record class DeliverOrderCommand
(
    Guid OrderId
): IRequest<Result<Unit>>;
