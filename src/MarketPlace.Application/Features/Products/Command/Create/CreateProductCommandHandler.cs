using System.Collections.Generic;
using System.Linq;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
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

namespace MarketPlace.Application.Features.Products.Command.Create;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<ProductsListItemDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _category_repository;
    private readonly IVendorRepository _vendor_repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly ICurrentUser _currentUser;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IVendorRepository vendorRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<CreateProductCommandHandler> logger
    )
    {
        _productRepository = productRepository;
        _category_repository = categoryRepository;
        _vendor_repository = vendorRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductsListItemDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken
    )
    {
        // Request already carries strongly-typed value objects
        var categoryIds = request.CategoryIds.Select(CategoryId.Create).ToList(); //gets the category ids from the request and converts them to CategoryId value objects

        var currentUserId = _currentUser.GetCurrentUserId();
        Console.WriteLine($"CurrentUserId: {currentUserId}"); // Log the current user id for debugging
        if (currentUserId == Guid.Empty)
        {
            return Result<ProductsListItemDto>.Failure(UserError.UserNotAuthenticated());
        }
        var vendor = await _vendor_repository.GetVendorUserIdAsync(currentUserId, cancellationToken);
        // var vendorId = VendorId.Create(currentUserId); // assuming the vendorId is the same as the current user's id
        // Console.WriteLine($"VendorId: {vendorId.Value}"); // Log the vendorId for debugging
        if (vendor is null)
        {
            return Result<ProductsListItemDto>.Failure(VendorError.VendorNotFoundOrInactive());
        }
        var validCategoryCount = await _category_repository.CountValidLeafCategoriesAsync(
            categoryIds,
            cancellationToken
        );
        if (validCategoryCount != categoryIds.Count)
            return Result<ProductsListItemDto>.Failure(CategoryError.InvalidOrNonLeafCategories());

        var activeVendor = await _vendor_repository.GetActiveByIdAsync(vendor.Id, cancellationToken);
        Console.WriteLine($"Vendor: {vendor?.Id}"); // Log the vendor for debugging
        if (activeVendor is null)
            return Result<ProductsListItemDto>.Failure(VendorError.VendorNotFoundOrInactive());

        if (vendor.UserId != currentUserId)
             return Result<ProductsListItemDto>.Failure(VendorError.VendorNotOwnedByUser());

        var images =
            request
                .ImageUrl?.Where(url => !string.IsNullOrWhiteSpace(url))
                .Select(url => Img.Create(url))
                .ToList()
            ?? new List<Img>();

        var tags =
            request
                .Tags?.Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => Tag.Create(tag))
                .ToList()
            ?? new List<Tag>();

        var result = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.Sku,
            images,
            tags,
            activeVendor.Id,
            categoryIds
        );

        if (result.IsFailure)
            return Result<ProductsListItemDto>.Failure(result.Error);

        var product = result.Value;
        await _productRepository.AddAsync(product, cancellationToken);
        product.Publish(); // Publish the product after adding it to the repository
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Product created successfully with Id: {ProductId}", product.Id);

        return Result<ProductsListItemDto>.Success(
            new ProductsListItemDto(
                product.Id.Value,
                product.Name,
                product.Price,
                product.Images.Select(i => i.Url).ToList(),
                product.Status.ToString(),
                product.Description,
                product.Tags.Select(t => t.Name).ToList(),
                product.Sku,
                product.StockQuantity
            )
        );
    }
}
