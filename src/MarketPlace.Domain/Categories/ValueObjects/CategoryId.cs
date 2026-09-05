using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Categories.ValueObjects;


  public sealed class CategoryId : ValueObject
    {
        public Guid Value { get; }
        private CategoryId(Guid value)
        {
            Value = value;
        }
        public static CategoryId Create() => new(Guid.NewGuid());
        public static CategoryId Create(Guid guid) => new(guid);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}