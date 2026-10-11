using FluentValidation;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Categories.ValueObjects;

namespace MarketPlace.Application.Features.Categories.Command;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Category description must not exceed 500 characters.");

        _ = RuleFor(x => x.ParentCategoryId)
            .MustAsync(async (parentId, cancellationToken) =>
            {
                if (parentId == null) return true;
                return await _categoryRepository.ExistsAsync(CategoryId.Create(parentId.Value), cancellationToken);
            })
            .WithMessage("Parent category does not exist.");
    }
}


