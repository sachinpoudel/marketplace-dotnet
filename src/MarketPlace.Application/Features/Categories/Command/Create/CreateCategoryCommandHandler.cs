using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Categories.Dtos;
using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Categories.Command;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryCommandHandler> logger
) : IRequestHandler<CreateCategoryCommand, Result<CategoryDetailDto>>
{
 public async Task<Result<CategoryDetailDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
{
    CategoryId? parentCategoryId = request.ParentCategoryId.HasValue 
        ? CategoryId.Create(request.ParentCategoryId.Value) 
        : null;

    if (parentCategoryId is not null)
    {
        var parentExists = await categoryRepository.ExistsAsync(parentCategoryId, cancellationToken);
        if (!parentExists)
            return Result<CategoryDetailDto>.Failure(CategoryError.ParentCategoryDoesNotExist());
    }

    var category = Category.Create(request.Name, request.Description, parentCategoryId);

    if (category.IsFailure)
    {
        logger.LogWarning("Failed to create category: {Error}", category.Error);
        return Result<CategoryDetailDto>.Failure(category.Error);
    }

    await categoryRepository.AddAsync(category.Value, cancellationToken);
    await unitOfWork.CommitAsync(cancellationToken);

    logger.LogInformation("Category created successfully with Id: {CategoryId}", category.Value.Id);

    return Result<CategoryDetailDto>.Success(new CategoryDetailDto(
        category.Value.Id.Value, 
        category.Value.Name,
        category.Value.Description,
        category.Value.ParentCategoryId?.Value 
    ));
}

}
        