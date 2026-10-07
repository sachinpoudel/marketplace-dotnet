using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Deliver;

public class DeliverOrderCommandHandler(ICurrentUser currentUser, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeliverOrderCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == Guid.Empty)
        {
            return Result<Unit>.Failure(UserError.UserNotAuthenticated()); 
        }
        var order = await orderRepository.GetOrderByIdAsync(OrderId.Create(request.OrderId), cancellationToken);
        if (order == null)
        {
            return Result<Unit>.Failure(OrderError.OrderNotFound());
        }
        if (order.UserId != currentUserId)
        {
            return Result<Unit>.Failure(UserError.UserNotAuthorized());
        }
        order.MarkAsDelivered();
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}