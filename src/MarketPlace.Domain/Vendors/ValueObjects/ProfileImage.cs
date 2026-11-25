using System;
using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Vendors.ValueObjects;

public class ProfileImage :  ValueObject
{
    public string Url { get; private set; }
   

    public ProfileImage(string url)
    {
        Url = url;
    }
    public static ProfileImage Create(string url) => new ProfileImage(url);
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Url;
    }


}
