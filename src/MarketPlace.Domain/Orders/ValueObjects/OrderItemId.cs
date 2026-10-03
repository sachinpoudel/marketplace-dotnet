using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Orders.ValueObjects;

public class OrderItemId : ValueObject
{
   public Guid Value {get;private set;}
    private OrderItemId(Guid value)
    {
        Value = value;
    }
    public static OrderItemId Create () => new(Guid.NewGuid());
    public static OrderItemId Create (Guid value) => new(value);
   

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
