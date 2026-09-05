using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Common.ValueObjects;


public class Img : ValueObject
{
    public string Url { get; private set; }

    private Img(string url)
    {
        Url = url;
    }
    public static Img Create(string url) => new Img(url);
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Url;
    }
}