using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Products.Command.Create;


public class CreateProductCommandHandler(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger
) : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    public async Task<Result<ProductId>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // to create product logicc in handler we have to
        // 1. check if category exists
        // 2. check if vendor exists
        // 3. create product
        // 4. return product id

        foreach (var categoryId in request.CategoryIds)
        {
            var categoryExists = await categoryRepository.ExistsAsync(categoryId, cancellationToken);

            if (!categoryExists)
            {
                return Result<ProductId>.Failure(CategoryError.CategoryDoesNotExist());
            }
        }
        var hasSubCategories = await categoryRepository.HasChildrenAsync(request.CategoryIds.FirstOrDefault(), cancellationToken);
        if (hasSubCategories)
            return Result<ProductId>.Failure(CategoryError.CategoryMustBeLeaf());
        var vendor = await vendorRepository.GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor is null)
        {
            return Result<ProductId>.Failure(VendorError.VendorNotFound());
        }
        if (vendor.Status != VendorStatus.Active)
            return Result<ProductId>.Failure(VendorError.VendorNotActive());
        var result = Product.Create(request.Name, request.Description, request.Price, request.StockQuantity, request.Sku, request.ImageUrl, request.Tags, request.VendorId, request.CategoryIds);

        if (result.IsFailure)
        {
            return Result<ProductId>.Failure(result.Error);
        }
        var product = result.Value;
        var addResult = await productRepository.AddAsync(product, cancellationToken);
        if (addResult is null)
        {
            logger.LogInformation("Failed to add product");
            return Result<ProductId>.Failure(ProductError.ProductCreationFailed());

        }
        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Product created successfully with Id: {ProductId}", product.Id);
        return Result<ProductId>.Success(product.Id);
    }
}