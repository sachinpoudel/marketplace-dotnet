using System;
using FluentValidation;

namespace MarketPlace.Application.Features.Orders.Command.Confirm;

public class ConfirmOrderValidator : AbstractValidator<ConfirmOrderCommand>
{
  public ConfirmOrderValidator()
  {
    RuleFor(x => x.OrderId)
      .NotEmpty().WithMessage("OrderId is required.");
  }
}
