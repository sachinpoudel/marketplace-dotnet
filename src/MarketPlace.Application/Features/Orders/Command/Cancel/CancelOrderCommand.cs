using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Cancel;

public record class CancelOrderCommand
(
    Guid OrderId
): IRequest<Result<CancelOrderDto>>;
