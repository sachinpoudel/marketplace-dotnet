using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Payments.ValueObjects;

public sealed class PaymentId : ValueObject
{
    public Guid Value {get;private set;}
    private PaymentId(Guid value)
    {
        Value = value;
    }
    public static PaymentId Create () => new(Guid.NewGuid());
    public static PaymentId Create (Guid value) => new(value);
   

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
   
}
