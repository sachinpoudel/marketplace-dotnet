using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common;
using MarketPlace.Domain.Products.Entities;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.Entities;
using MarketPlace.Domain.Reviews.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Infrastructure.Persistence.Repositories;


public class ReviewRepository(ApplicationDbContext context) : IReviewRepository
{
    public async Task<Review> AddReviewAsync(Review review)
    {
    var reviews = await context.Reviews.AddAsync(review);
return reviews.Entity;
        

    }

    public async Task DeleteReviewAsync(Review review)
    {
         context.Reviews.Remove(review);
    }

  

    public async Task<Review?> GetReviewByIdAsync(ReviewId id, CancellationToken cancellationToken)
    {
        return await context.Reviews.FindAsync(id);
    }

    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(ProductId productId)
    {
         return await context.Reviews.Where(r => r.ProductId == productId && !r.IsDeleted).ToListAsync();
    }

    public async Task UpdateReviewAsync(Review review)
    {
         context.Reviews.Update(review);
    }
}