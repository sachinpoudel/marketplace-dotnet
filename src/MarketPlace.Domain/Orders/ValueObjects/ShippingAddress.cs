using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Orders.ValueObjects;

public class ShippingAddress :ValueObject
{
 public string FullName {get; private set;} = default!;
    public string AddressLine1 {get; private set;} = default!;
    public string AddressLine2 {get; private set;} = default!;
    public string City {get; private set;} = default!;
    public string State {get; private set;} = default!;
    public string PhoneNumber {get; private set;} = default!;

    private ShippingAddress(string fullName, string addressLine1, string addressLine2, string city, string state, string phoneNumber)
    {
        FullName = fullName;
        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        PhoneNumber = phoneNumber;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FullName;
        yield return AddressLine1;
        yield return AddressLine2;
        yield return City;
        yield return State;
        yield return PhoneNumber;
    }
}
