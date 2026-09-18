using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Vendors.Command;



public class CreateVendorCommandHandler(
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateVendorCommandHandler> logger,

IAuthService authService,

    ICurrentUser currentUser) : IRequestHandler<CreateVendorCommand, Result<VendorDetailDto>>
{
    public async Task<Result<VendorDetailDto>> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
    {
        // to create vendor logic in handler we have to
        // 1. validate the request
        // 2. create vendor
        // 3. return vendor detail
        var vendorExists = await vendorRepository.ExistsAsync( request.ContactEmail, cancellationToken);


        if (vendorExists)
        {
            return Result<VendorDetailDto>.Failure(VendorError.VendorAlreadyExists());
        }

        var currentUserId = currentUser.UserId;

        if (currentUserId == null)
            return Result<VendorDetailDto>.Failure(UserError.UserNotAuthenticated());



        var userExist = await authService.IsUserExists(currentUserId, cancellationToken);

if(!userExist)
        {
            return Result<VendorDetailDto>.Failure(UserError.UserNotFound());
        }
        var convertedUserId = Guid.Parse(currentUserId);
 
        var result = Vendor.Create(convertedUserId,request.LegalName, request.TradeName, request.Description, request.ProfileUrl ?? string.Empty, request.BusinessAddress, request.ContactEmail);

        var vendor = result.Value;

        var addResult = await vendorRepository.AddAsync(vendor, cancellationToken);
        if (addResult is null)

        {
            return Result<VendorDetailDto>.Failure(result.Error);
        }
        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Vendor created successfully with Id: {VendorId}", result.Value.Id);
        return Result<VendorDetailDto>.Success(new VendorDetailDto(
            LegalName: result.Value.LegalName,
            TradeName: result.Value.TradeName,
            Description: result.Value.Description,
            ProfileUrl: result.Value.ProfileUrl,
            Email: result.Value.ContactEmail,
            BusinessAddress: result.Value.BusinessAddress
        ));

    }
}
