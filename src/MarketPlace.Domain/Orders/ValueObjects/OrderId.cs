using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Orders.ValueObjects;

public class OrderId : ValueObject
{
   public Guid Value {get;private set;}
    private OrderId(Guid value)
    {
        Value = value;
    }
    public static OrderId Create () => new(Guid.NewGuid());
    public static OrderId Create (Guid value) => new(value);
   

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
