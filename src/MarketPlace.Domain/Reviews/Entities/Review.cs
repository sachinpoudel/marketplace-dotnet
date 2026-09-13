using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.ValueObjects;

namespace MarketPlace.Domain.Reviews.Entities;

public class Review: AggregateRoot<ReviewId>
{
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public double Rating { get; private set; }
    public ProductId ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private Review(ReviewId id, Guid userId, string content, double rating, ProductId productId) : base(id)

    {
        Id = id;
        UserId = userId;
        Content = content;
        Rating = rating;
        ProductId = productId;
        CreatedAt = DateTime.UtcNow;
    }
    private Review() { }
    public static Review Create(ReviewId id, Guid userid, string content, double rating, ProductId productId)
    {
        return new Review(id, userid, content, rating, productId);
    }
    public void Update(string content, double rating)
    {
        Content = content;
        Rating = rating;
    }

   
}