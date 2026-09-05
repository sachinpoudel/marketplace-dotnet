using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Reviews.ValueObjects;


public sealed class ReviewsId : ValueObject
{
    public Guid Value {get;private set;}
    private ReviewsId(Guid value)
    {
        Value = value;
    }
    public static ReviewsId Create () => new(Guid.NewGuid());
    public static ReviewsId Create (Guid value) => new(value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}