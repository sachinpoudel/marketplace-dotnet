using FluentValidation;

namespace MarketPlace.Application.Features.Vendors.Command.Create;


public class CreateVendorValidator : AbstractValidator<CreateVendorCommand>
{
    public CreateVendorValidator()
    {
        RuleFor(v => v.LegalName)
            .NotEmpty().WithMessage("Legal name is required.")
            .MaximumLength(100).WithMessage("Legal name must not exceed 100 characters.");

        RuleFor(v => v.TradeName)
            .NotEmpty().WithMessage("Trade name is required.")
            .MaximumLength(100).WithMessage("Trade name must not exceed 100 characters.");

        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

      

        RuleFor(v => v.BusinessAddress)
            .NotEmpty().WithMessage("Business address is required.")
            .MaximumLength(200).WithMessage("Business address must not exceed 200 characters.");

        RuleFor(v => v.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Contact email must be a valid email address.");
    }
}