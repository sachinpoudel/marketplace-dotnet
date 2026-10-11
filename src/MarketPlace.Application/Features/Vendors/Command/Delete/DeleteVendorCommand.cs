using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command.Delete;

public record class DeleteVendorCommand
(
    Guid VendorId
) : IRequest<Result<bool>>;
