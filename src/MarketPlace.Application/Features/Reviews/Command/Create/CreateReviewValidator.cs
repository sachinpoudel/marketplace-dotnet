using FluentValidation;

namespace MarketPlace.Application.Features.Reviews.Command.Create;

public class CreateReviewValidator : AbstractValidator<CreateReviewcommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MinimumLength(1).MaximumLength(100);
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");


    }
}
