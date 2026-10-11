using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command.Update;

public record class UpdateVendorCommand
(
  Guid VendorId, 
    string LegalName,
    string TradeName,
    string Description,
   
    string BusinessAddress
   
) : IRequest<Result<VendorDetailDto>>;
