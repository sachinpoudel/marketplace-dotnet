using System;
using FluentValidation;

namespace MarketPlace.Application.Features.Carts.Commands.AddItem;

public class AddCartItemValidator : AbstractValidator<AddCartItemCommand>
{
 public AddCartItemValidator()
 {
  RuleFor(x => x.Quantity)
   .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
   .LessThanOrEqualTo(100).WithMessage("Quantity must be less than or equal to 100.");
 }
}
