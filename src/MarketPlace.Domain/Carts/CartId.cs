using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Carts;

public class CartId :ValueObject
{
  private CartId() { } // Required for EF Core
        public Guid Value { get; }
        private CartId(Guid value)
        {
            Value = value;
        }
        public static CartId Create() => new(Guid.NewGuid());
        public static CartId Create(Guid guid) => new(guid);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
