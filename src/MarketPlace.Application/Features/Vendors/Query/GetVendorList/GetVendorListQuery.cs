using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Vendors.Dtos;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Query.GetVendorList;

public record class GetVendorListQuery


(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null
): IRequest<PaginatedList<VendorDetailDto>>;

