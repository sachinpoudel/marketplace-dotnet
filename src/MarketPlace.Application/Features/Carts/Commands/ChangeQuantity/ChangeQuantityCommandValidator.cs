using System;
using FluentValidation;

namespace MarketPlace.Application.Features.Carts.Commands.ChangeQuantity;

public class ChangeQuantityCommandValidator : AbstractValidator<ChangeQuantityCommand>
{
    public ChangeQuantityCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
            RuleFor(x => x.CartId)
            .NotEmpty()
            .WithMessage("CartId is required.");
            RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");
    }
}
