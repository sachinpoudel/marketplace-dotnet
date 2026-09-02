using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.Entities;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IVendorRepository
{
    Task<Result<Vendor>> GetByIdAsync(Guid vendorId);
    Task<Result<Vendor>> AddAsync(Vendor vendor);
    Task<Result<Vendor>> GetByVendorIdAsync(Guid vendorId);
    
}