using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Categories.Command;

public class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryCommandHandler> logger
) : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await categoryRepository.ExistsAsync(request.ParentCategoryId.Value, cancellationToken);
            if (!parentExists)
                return Result<Guid>.Failure(CategoryError.ParentCategoryDoesNotExist());
        }

        var category = Category.Create(request.Name, request.Description, request.ParentCategoryId);

        if (category.IsFailure)
        {
            logger.LogWarning("Failed to create category: {Error}", category.Error);
            return Result<Guid>.Failure(category.Error);
        }

        await categoryRepository.AddAsync(category.Value, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Category created successfully with Id: {CategoryId}", category.Value.Id);

        return Result<Guid>.Success(category.Value.Id);
    }
}