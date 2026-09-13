using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.Entities;
using MarketPlace.Domain.Reviews.ValueObjects;

namespace MarketPlace.Infrastructure.Persistence.Repositories;


public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task<Review> AddReviewAsync(Review review)
    {
        // return await context.Reviews.AddAsync(review);

        throw new NotImplementedException();

    }

    public Task DeleteReviewAsync(Review review)
    {
        throw new NotImplementedException();
    }

    public Task<Review?> GetReviewByIdAsync(ReviewId id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Review>> GetReviewsByProductIdAsync(ProductId productId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateReviewAsync(Review review)
    {
        throw new NotImplementedException();
    }
}