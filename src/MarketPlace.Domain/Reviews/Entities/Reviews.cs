using MarketPlace.Domain.Common.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.ValueObjects;

namespace MarketPlace.Domain.Reviews.Entities;

public class Reviews: AggregateRoot<ReviewsId>
{
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public ProductId ProductId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Reviews(ReviewsId id, string content, int rating, ProductId productId) : base(id)
    {
        Id = id;
        Content = content;
        Rating = rating;
        ProductId = productId;
        CreatedAt = DateTime.UtcNow;
    }
    public Reviews() { }
    public static Reviews Create(ReviewsId id, string content, int rating, ProductId productId)
    {
        return new Reviews(id, content, rating, productId);
    }
    public void Update(string content, int rating)
    {
        Content = content;
        Rating = rating;
    }
}