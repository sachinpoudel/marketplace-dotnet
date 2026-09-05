using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Products.ValueObjects;


public sealed class ProductId : ValueObject
{
    public Guid Value {get;private set;}
    private ProductId(Guid value)
    {
        Value = value;
    }
    public static ProductId Create () => new(Guid.NewGuid());
    public static ProductId Create (Guid value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}