using System;
using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Orders.Command.Create;

public record CreateOrderCommand(
 Guid CartId,
    string FullName,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PhoneNumber
  
) : IRequest<Result<OrderDetailDto>>;

