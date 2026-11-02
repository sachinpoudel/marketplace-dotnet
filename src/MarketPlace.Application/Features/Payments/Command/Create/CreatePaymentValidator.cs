using FluentValidation;

namespace MarketPlace.Application.Features.Payments.Command.Create;

public class CreatePaymentValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("Order Id is required.");
        RuleFor(x => x.Method).IsInEnum().WithMessage("Invalid payment method.");
    }
}
