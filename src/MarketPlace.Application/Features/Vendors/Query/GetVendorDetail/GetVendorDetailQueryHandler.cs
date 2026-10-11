using System;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Query.GetVendorDetail;

public class GetVendorDetailQueryHandler(
    ICurrentUser currentUser,
    IVendorRepository vendorRepository
) : IRequestHandler<GetVendorDetailQuery, Result<VendorDetailDto>>
{
    public async Task<Result<VendorDetailDto>> Handle(
        GetVendorDetailQuery request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if(currentUserId == null)
        {
            return Result<VendorDetailDto>.Failure(UserError.UserNotAuthenticated());
        }

        var vendor = await vendorRepository.GetByIdAsync(
            VendorId.Create(request.VendorId),
            cancellationToken
        );
        if (vendor == null)
        {
            return Result<VendorDetailDto>.Failure(VendorError.VendorNotFoundOrInactive());
        }

    return Result<VendorDetailDto>.Success(
    new VendorDetailDto(
        vendor.Id.Value,
        vendor.LegalName,
        vendor.TradeName,
        vendor.Description,
        vendor.ProfileUrl,
        vendor.ContactEmail,
        vendor.BusinessAddress
    )
);
    }
}
