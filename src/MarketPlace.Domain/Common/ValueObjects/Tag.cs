using MarketPlace.Domain.Common.Entities;

namespace MarketPlace.Domain.Common.ValueObjects;
public class Tag : ValueObject

{
    public string Name { get; private set; }
    public string Slug { get; private set; }

    private Tag() { } // Required for EF Core

    public Tag(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));

        Name = name.Trim().ToLowerInvariant();
        Slug = Name.Replace(" ", "-");
    }
public static Tag Create(string name)
    {
        return new Tag(name);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}