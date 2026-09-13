using MarketPlace.Application.Common.Interfaces.Query;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    IProductQuery productQuery
) : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{




    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {

        var productId = ProductId.Create(request.Id);

        var product = await productQuery.GetProductDetailAsync(productId, cancellationToken);

        if (product == null)
        {
            return Result<ProductDetailDto>.Failure(ProductError.ProductNotFound());
        }

        return Result<ProductDetailDto>.Success(new ProductDetailDto
        (
             product.Id,
             product.Name,
            product.Description,
             product.Price,
            product.StockQuantity,
             product.Sku,
       product.ImageUrl.Select(i => i).ToList(),
             product.Tags.Select(t => t).ToList(),
            product.CategoryIds.Select(c => c).ToList(),
            product.VendorId,
            product.Reviews


        ));
    }
}