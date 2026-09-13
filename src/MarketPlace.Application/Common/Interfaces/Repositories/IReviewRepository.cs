using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.Entities;
using MarketPlace.Domain.Reviews.ValueObjects;

namespace MarketPlace.Application.Common.Interfaces.Repositories;


public interface IReviewRepository
{
    Task<Review?> GetReviewByIdAsync(ReviewId id);
    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(ProductId productId);
    Task<Review> AddReviewAsync(Review review);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewAsync(Review review);
}