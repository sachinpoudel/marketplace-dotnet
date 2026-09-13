using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Reviews.ValueObjects;


public sealed class ReviewId : ValueObject
{
    public Guid Value {get;private set;}
    private ReviewId(Guid value)
    {
        Value = value;
    }
    public static ReviewId Create () => new(Guid.NewGuid());
    public static ReviewId Create (Guid value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}