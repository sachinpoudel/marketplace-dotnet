using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Vendors.Enums;
using MarketPlace.Domain.Vendors.Events;
using MarketPlace.Domain.Vendors.ValueObjects;

namespace MarketPlace.Domain.Vendors.Entities;


public sealed class Vendor : AggregateRoot<VendorId>
{
    public string LegalName { get; private set; } = string.Empty;
    public string TradeName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string? ProfileUrl { get; private set; } = string.Empty;


    public VendorStatus Status { get; private set; } = VendorStatus.Inactive;

    public string BusinessAddress { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;




    private Vendor(
     VendorId id,
     string legalName,
     string tradeName,
     string description,
     string profileUrl,
     VendorStatus status,
     string businessAddress,
     string contactEmail) : base(id)
    {
        LegalName = legalName;
        TradeName = tradeName;
        Description = description;
        ProfileUrl = profileUrl;
        Status = status;
        BusinessAddress = businessAddress;
        ContactEmail = contactEmail;
    }

    private Vendor() { }

    public static Result<Vendor> Create(
       
      string legalName, string tradeName, string description,
      string profileUrl, string businessAddress, string contactEmail) // status param removed
    {
        if (string.IsNullOrWhiteSpace(legalName) && string.IsNullOrWhiteSpace(tradeName))
            return Result<Vendor>.Failure(VendorError.VendorNameIsRequired());

        if (contactEmail == null || !contactEmail.Contains("@"))
            return Result<Vendor>.Failure(VendorError.VendorContactEmailIsInvalid());

        var vendor = new Vendor(VendorId.Create(), legalName, tradeName, description,
            profileUrl, VendorStatus.Pending, businessAddress, contactEmail);

        vendor.AddDomainEvent(new VendorRegisteredEvent(vendor.Id));
        return Result<Vendor>.Success(vendor);
    }



    public Result UpdateProfile(string profileUrl)
    {
        if (string.IsNullOrWhiteSpace(profileUrl))
            return  Result.Failure(VendorError.VendorProfileUrlIsInvalid());

        ProfileUrl = profileUrl;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }



    public Result UpdateDetails(string legalName, string tradeName, string description, string businessAddress)
    {
        if (string.IsNullOrWhiteSpace(legalName) && string.IsNullOrWhiteSpace(tradeName))
        {
            return Result.Failure(VendorError.VendorNameIsRequired());
        }

        LegalName = legalName;
        TradeName = tradeName;
        Description = description;
        BusinessAddress = businessAddress;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Publish()
    {
        if (Status != VendorStatus.Pending)
        {
            return Result.Failure(VendorError.InvalidStatusTransition());
        }

        Status = VendorStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
            
    public Result Delist()
    {
        Status = VendorStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new VendorDelistedEvent(Id));
        return Result.Success();
    }
}