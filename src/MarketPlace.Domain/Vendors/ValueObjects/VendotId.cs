using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Vendors.ValueObjects;


public sealed class VendorId : ValueObject
{
    public Guid Value {get;private set;}
    private VendorId(Guid value)
    {
        Value = value;
    }
    public static VendorId Create () => new(Guid.NewGuid());
    public static VendorId Create (Guid value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}