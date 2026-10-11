using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command.Delete;

public class DeleteVendorCommandHandler(ICurrentUser currentUser, IVendorRepository vendorRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteVendorCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result<bool>.Failure(UserError.UserNotAuthenticated());
        }

        var vendor = await vendorRepository.GetByIdAsync(VendorId.Create(request.VendorId), cancellationToken);
        if (vendor == null)
        {
            return Result<bool>.Failure(VendorError.VendorNotFoundOrInactive());
        }

        if (vendor.UserId != currentUserId)
        {
            return Result<bool>.Failure(VendorError.VendorNotOwnedByUser());
        }

        vendor.Delete();
        await vendorRepository.UpdateAsync(vendor, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return  Result<bool>.Success(true);
    }
}
