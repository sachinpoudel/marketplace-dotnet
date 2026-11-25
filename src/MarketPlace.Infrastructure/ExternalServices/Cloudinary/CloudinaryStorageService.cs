using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MarketPlace.Application.Common.Interfaces.ImageStorage;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using Microsoft.Extensions.Options;

namespace MarketPlace.Infrastructure.ExternalServices.Cloudinary;

public class CloudinaryStorageService : IImageStorage
{
    public readonly CloudinaryDotNet.Cloudinary _cloudinary;

    public CloudinaryStorageService(IOptions<CloudinarySettings> options)
    {
        var account = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );
        _cloudinary = new CloudinaryDotNet.Cloudinary(account);
    }

    public Task DeleteImageAsync(string publicId, CancellationToken cancellationToken)
    {
        var deleteParmas = new DeletionParams(publicId);
        var result = _cloudinary.Destroy(deleteParmas);
        return Task.FromResult(result);
    }

    public async Task<Result<Application.Features.Vendors.Dtos.ImageUploadResult>> UploadImageAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken
    )
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, stream),
            Folder = "profile_pictures",
            Transformation = new Transformation()
                .Width(200)
                .Height(200)
                .Crop("fill")
                .Gravity("face"),
        };
        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
        if (result is null)
        {
            return Result<Application.Features.Vendors.Dtos.ImageUploadResult>.Failure(UserError.FailedToUploadProfilePicture());
        }
        else
        {
            return Result<Application.Features.Vendors.Dtos.ImageUploadResult>.Success(new Application.Features.Vendors.Dtos.ImageUploadResult(result.SecureUrl.ToString(), result.PublicId));
        }
    }

  
}
