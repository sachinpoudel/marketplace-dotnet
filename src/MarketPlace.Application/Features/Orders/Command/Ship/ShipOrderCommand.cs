using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Ship;

public record class ShipOrderCommand
(
    Guid OrderId
): IRequest<Result<Unit>>;
