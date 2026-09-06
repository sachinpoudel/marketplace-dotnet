using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Vendors.Entities;
using MarketPlace.Domain.Vendors.Enums;
using MarketPlace.Domain.Vendors.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;

public sealed class VendorRepository(ApplicationDbContext context) : IVendorRepository
{
    public async Task<Vendor?> GetByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default)
    {
        return await context.Vendors
            .FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);
    }

    public async Task<Vendor> AddAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        await context.Vendors.AddAsync(vendor, cancellationToken);
        return vendor;
    }

    public async Task<Vendor?> GetActiveByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default)
    {
        return await context.Vendors
            .FirstOrDefaultAsync(v => v.Id == vendorId && v.Status == VendorStatus.Active, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string legalName, string tradeName, string contactEmail, CancellationToken cancellationToken = default)
    {
        return await context.Vendors.AnyAsync(v =>
            v.LegalName == legalName &&
            v.TradeName == tradeName &&
            v.ContactEmail == contactEmail,
            cancellationToken);
    }
}
