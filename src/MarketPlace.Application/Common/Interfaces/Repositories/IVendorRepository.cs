using MarketPlace.Domain.Categories.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Vendors.Entities;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default);
    Task<Vendor> AddAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string legalName, string tradeName, string contactEmail, CancellationToken cancellationToken = default);
}