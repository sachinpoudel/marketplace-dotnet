using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command.Create;

public record class ImageUploadCommand
(
    Stream ImageStream,
    string FileName
) : IRequest<Result<string>>;
