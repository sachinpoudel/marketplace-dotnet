using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Query.GetVendorDetail;

public record class GetVendorDetailQuery
(
    Guid VendorId
): IRequest<Result<VendorDetailDto>>;