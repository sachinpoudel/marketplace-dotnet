using MarketPlace.Application.Features.Vendors.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Vendors.Command;



public record CreateVendorCommand(string LegalName, string TradeName, string Description,
    string? ProfileUrl, string BusinessAddress, string ContactEmail) : IRequest<Result<VendorDetailDto>>;