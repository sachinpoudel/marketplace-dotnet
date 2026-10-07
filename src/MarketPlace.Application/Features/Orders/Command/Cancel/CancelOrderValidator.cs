using System;
using FluentValidation;

namespace MarketPlace.Application.Features.Orders.Command.Cancel;

public class CancelOrderValidator : AbstractValidator<CancelOrderCommand>
{
  public CancelOrderValidator()
  {
    RuleFor(x => x.OrderId)
      .NotEmpty().WithMessage("OrderId is required.");
  }
}
