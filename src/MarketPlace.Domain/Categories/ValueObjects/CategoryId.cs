using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Categories.ValueObjects;


  public sealed class CategoryId : ValueObject
    {
        private CategoryId() { } // Required for EF Core
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