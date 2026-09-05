namespace MarketPlace.Application.Features.Vendors.Dtos;


public record VendorDetailDto(
    string LegalName,
    string TradeName,
    string Description,
    string ProfileUrl,
   
    string Email,
  
    string BusinessAddress
   
);