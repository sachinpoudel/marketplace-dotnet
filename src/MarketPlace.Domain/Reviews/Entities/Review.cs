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
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    private Review(ReviewId id, Guid userId, string content, double rating, ProductId productId, DateTime? updatedAt) : base(id)

    {
        Id = id;
        UserId = userId;
        Content = content;
        Rating = rating;
        ProductId = productId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = updatedAt;
    }
    private Review() { }
    public static Review Create(ReviewId id, Guid userid, string content, double rating, ProductId productId, DateTime? updatedAt = null)
    {
        return new Review(id, userid, content, rating, productId, updatedAt);
    }
    public void Update(string content, double rating, DateTime? updatedAt = null)
    {
        Content = content;
        Rating = rating;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }
    public void Delete()
    {
        if(IsDeleted)
        {
            throw new InvalidOperationException("Review is already deleted.");
        }
        IsDeleted = true;
    }
 


   
}