using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Common.ValueObjects;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.Enums;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

namespace MarketPlace.Application.Features.Products.Command.Create;


public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductsListItemDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _category_repository;
    private readonly IVendorRepository _vendor_repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _category_repository = categoryRepository;
        _vendor_repository = vendorRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

   public async Task<Result<ProductsListItemDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Request already carries strongly-typed value objects
        var categoryIds = request.CategoryIds
            .Select(CategoryId.Create)
            .ToList();

        var vendorId = VendorId.Create(request.VendorId);
        
        var validCategoryCount = await _category_repository.CountValidLeafCategoriesAsync(categoryIds, cancellationToken);
        if (validCategoryCount != categoryIds.Count)
            return Result<ProductsListItemDto>.Failure(CategoryError.InvalidOrNonLeafCategories());

        var vendor = await _vendor_repository.GetActiveByIdAsync(vendorId, cancellationToken);
        if (vendor is null)
            return Result<ProductsListItemDto>.Failure(VendorError.VendorNotFoundOrInactive());

        var images = string.IsNullOrWhiteSpace(request.ImageUrl)
            ? Enumerable.Empty<Img>()
            : new List<Img> { Img.Create(request.ImageUrl!) };

        var result = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.Sku,
            images,
            request.Tags,
            vendorId,
            categoryIds);

        if (result.IsFailure)
            return Result<ProductsListItemDto>.Failure(result.Error);

        var product = result.Value;
        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with Id: {ProductId}", product.Id);

        return Result<ProductsListItemDto>.Success(new ProductsListItemDto(
            product.Id,
            product.Name,
            product.Price,
            product.Images.FirstOrDefault()?.Url ?? string.Empty,
            product.Status.ToString(),
            product.Description,
            product.Tags,
            product.Sku,
            product.StockQuantity
        ));
    }

}