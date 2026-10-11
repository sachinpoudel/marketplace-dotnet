using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command.Update;

public class UpdateVendorCommandHandler(
    ICurrentUser currentUser,
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateVendorCommand, Result<VendorDetailDto>>
{
    public async Task<Result<VendorDetailDto>> Handle(
        UpdateVendorCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result<VendorDetailDto>.Failure(UserError.UserNotAuthenticated());
        }

        var vendor = await vendorRepository.GetByIdAsync(
            VendorId.Create(request.VendorId),
            cancellationToken
        );
        if (vendor is null)
        {
            return Result<VendorDetailDto>.Failure(VendorError.VendorNotFoundOrInactive());
        }

        if (vendor.UserId != currentUserId)
        {
            return Result<VendorDetailDto>.Failure(VendorError.VendorNotOwnedByUser());
        }

        vendor.UpdateDetails(
            request.LegalName,
            request.TradeName,
            request.Description,
            request.BusinessAddress
        );
   await vendorRepository.UpdateAsync(vendor, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

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
