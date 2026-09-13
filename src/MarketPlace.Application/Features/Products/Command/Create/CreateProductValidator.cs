using FluentValidation;

namespace MarketPlace.Application.Features.Products.Command.Create;


public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required.")
            .MaximumLength(500).WithMessage("Product description must not exceed 500 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be zero or greater.");

        RuleFor(x => x.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU must not exceed 50 characters.");

RuleFor(x => x.Tags)
    .NotEmpty()
    .Must(tags => tags.Count <= 10)
    .WithMessage("A product can have a maximum of 10 tags.");

RuleForEach(x => x.Tags)
    .NotEmpty()
    .MaximumLength(50)
    .WithMessage("Each tag must not exceed 50 characters.");

RuleFor(x => x.ImageUrl)
    .Must(images => images == null || images.Count <= 10)
    .WithMessage("A product can have a maximum of 10 images.");

RuleForEach(x => x.ImageUrl)
    .NotEmpty()
    .MaximumLength(500)
    .WithMessage("Each image URL must not exceed 500 characters.");
    }
}