namespace MarketPlace.Application.Features.Vendors.Dtos;

public record VendorDetailDto(
    Guid Id,
    string LegalName,
    string TradeName,
    string Description,
    string? ProfileUrl,
    string Email,
    string BusinessAddress
);
