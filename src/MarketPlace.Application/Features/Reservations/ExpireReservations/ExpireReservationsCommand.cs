using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Orders.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Reservations.ExpireReservations;

public record class ExpireReservationsCommand (

 ): IRequest<Result>;


