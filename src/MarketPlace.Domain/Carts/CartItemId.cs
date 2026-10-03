using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Carts;

public class CartItemId :ValueObject
{
 private CartItemId() { } // Required for EF Core
        public Guid Value { get; }
        private CartItemId(Guid value)
        {
            Value = value;
        }
        public static CartItemId Create() => new(Guid.NewGuid());
        public static CartItemId Create(Guid guid) => new(guid);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
