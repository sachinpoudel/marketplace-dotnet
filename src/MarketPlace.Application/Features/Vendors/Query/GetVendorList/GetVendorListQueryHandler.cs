using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Common.Models;
using MarketPlace.Application.Features.Vendors.Dtos;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Query.GetVendorList;

public class GetVendorListQueryHandler(IVendorRepository vendorRepository) : IRequestHandler<GetVendorListQuery, PaginatedList<VendorDetailDto>>
{
    public Task<PaginatedList<VendorDetailDto>> Handle(GetVendorListQuery request, CancellationToken cancellationToken)
    {
        var query = vendorRepository.Query();


        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(v => v.TradeName.Contains(request.SearchTerm));

            var projected = query.OrderByDescending(v => v.CreatedAt).Select(v => new VendorDetailDto(
                v.Id.Value,
                v.LegalName,
                v.TradeName,
                v.Description,
                v.ProfileUrl,
                v.ContactEmail,
            
              v.BusinessAddress
            ));

        return projected.ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
