using System;
using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.ResultPattern;

namespace MarketPlace.Application.Common.Interfaces.ImageStorage;

public interface IImageStorage
{
  Task<Result<ImageUploadResult>> UploadImageAsync(Stream stream, string fileName, CancellationToken cancellationToken);
  Task DeleteImageAsync(string publicId, CancellationToken cancellationToken);
}
