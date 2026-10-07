using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Process;

public record class ProcessOrderCommand
(
    Guid OrderId
) : IRequest<Result<Unit>>;
