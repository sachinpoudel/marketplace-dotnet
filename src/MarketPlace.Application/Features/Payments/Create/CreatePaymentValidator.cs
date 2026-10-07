using System;
using FluentValidation;

namespace MarketPlace.Application.Features.Payments.Create;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
 public CreatePaymentValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Payment Id is required.");
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        RuleFor(x => x.Method).IsInEnum().WithMessage("Invalid payment method.");
        RuleFor(x => x.PaidAt).LessThanOrEqualTo(DateTime.UtcNow).WithMessage("PaidAt cannot be in the future.");
    }
}
