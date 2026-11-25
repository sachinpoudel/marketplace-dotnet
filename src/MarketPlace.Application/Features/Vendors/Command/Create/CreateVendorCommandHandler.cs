using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.ImageStorage;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Common.ValueObjects;
using MarketPlace.Domain.Vendors.Entities;
using MarketPlace.Domain.Vendors.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Vendors.Command.Create;

public class CreateVendorCommandHandler(
    IVendorRepository vendorRepository,
    IUnitOfWork unitOfWork,
    IImageStorage imageStorage,
    ILogger<CreateVendorCommandHandler> logger,
    IAuthService authService,
    ICurrentUser currentUser
) : IRequestHandler<CreateVendorCommand, Result<VendorDetailDto>>
{
public async Task<Result<VendorDetailDto>> Handle(
    CreateVendorCommand request,
    CancellationToken cancellationToken)
{
    var vendorExists = await vendorRepository.ExistsAsync(
        request.ContactEmail,
        cancellationToken);

    if (vendorExists)
        return Result<VendorDetailDto>.Failure(
            VendorError.VendorAlreadyExists());

    var currentUserId = currentUser.GetCurrentUserId();

    if (currentUserId == Guid.Empty)
        return Result<VendorDetailDto>.Failure(
            UserError.UserNotAuthenticated());

    var uploadedImageInput = request.ProfileUrl.FirstOrDefault();

    if (uploadedImageInput is null)
        return Result<VendorDetailDto>.Failure(
            VendorError.VendorProfileUrlIsInvalid());

    ImageUploadResult? uploadedImage = null;

    try
    {
        var vendorResult = Vendor.Create(
            currentUserId,
            request.LegalName,
            request.TradeName,
            request.Description,
            request.BusinessAddress,
            request.ContactEmail);

        if (vendorResult.IsFailure)
            return Result<VendorDetailDto>.Failure(vendorResult.Error);

        var vendor = vendorResult.Value;

        var uploadResult = await imageStorage.UploadImageAsync(
            uploadedImageInput.ImageStream,
            uploadedImageInput.FileName,
            cancellationToken);

        if (uploadResult.IsFailure)
            return Result<VendorDetailDto>.Failure(uploadResult.Error);

        uploadedImage = uploadResult.Value;

        var profileImageResult = ProfileImage.Create(uploadedImage.Url);

        if (profileImageResult is null)
            return Result<VendorDetailDto>.Failure(VendorError.VendorProfileUrlIsInvalid());

        vendor.UploadProfile(profileImageResult);

        await vendorRepository.AddAsync(vendor, cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Vendor created successfully with Id: {VendorId}",
            vendor.Id);

        return Result<VendorDetailDto>.Success(
            new VendorDetailDto(
                Id: vendor.Id.Value,
                LegalName: vendor.LegalName,
                TradeName: vendor.TradeName,
                Description: vendor.Description,
                ProfileUrl: vendor.ProfileUrl?.Url,
                Email: vendor.ContactEmail,
                BusinessAddress: vendor.BusinessAddress));
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "An error occurred while creating vendor for user {UserId}",
            currentUserId);

        if (uploadedImage is not null)
        {
            try
            {
                await imageStorage.DeleteImageAsync(
                    uploadedImage.PublicId,
                    CancellationToken.None);
            }
            catch (Exception cleanupException)
            {
                logger.LogError(
                    cleanupException,
                    "Failed to clean up Cloudinary image {PublicId}",
                    uploadedImage.PublicId);
            }
        }

        return Result<VendorDetailDto>.Failure(
            VendorError.VendorCreationFailed());
    }
    finally
    {
        uploadedImageInput.ImageStream?.Dispose();
    }
}}
