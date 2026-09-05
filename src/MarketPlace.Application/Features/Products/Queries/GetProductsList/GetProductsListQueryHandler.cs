using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Products.Dtos;
using MediatR;

namespace MarketPlace.Application.Features.Products.Queries.GetProductsList;

public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, PaginatedList<ProductsListItemDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsListQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedList<ProductsListItemDto>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
    {
        
        var query = _productRepository.Query();


        if(request.CategoryId is not null)
        {
            query = query.Where(p => p.CategoryIds.Contains(request.CategoryId));
        }
        if(request.VendorId is not null)
        {
            query = query.Where(p => p.VendorId == request.VendorId);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm));
 
 var projected =  query.OrderByDescending( p => p.CreatedAt).Select(p => new ProductsListItemDto(
                p.Id,
                p.Name,
                p.Price,
                p.ImageUrl ?? string.Empty,
                p.Status.ToString()
            ));

     return await projected.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}