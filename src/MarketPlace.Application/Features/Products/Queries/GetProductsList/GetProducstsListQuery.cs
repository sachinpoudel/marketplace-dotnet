using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Products.Dtos;
using MarketPlace.Domain.Categories.ValueObjects;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Products.Queries.GetProductsList;


public sealed record GetProductsListQuery (
    int PageNumber = 1,
    int PageSize = 10,
    CategoryId? CategoryId = null,
    VendorId? VendorId = null,
    string? SearchTerm = null,
    string? ImageUrl = null
): IRequest<PaginatedList<ProductsListItemDto>>;