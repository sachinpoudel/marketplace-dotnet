using MediatR;

namespace MarketPlace.Application.Features.Payments.Command.ProcessPendingRefunds;

public record class ProcessPendingRefundsCommand : IRequest;
