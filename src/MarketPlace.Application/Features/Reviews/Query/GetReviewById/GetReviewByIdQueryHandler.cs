using System;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Reviews.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Query.GetReviewById;

public class GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
    : IRequestHandler<GetReviewByIdQuery, Result<ReviewDetailDto>>
{
    public async Task<Result<ReviewDetailDto>> Handle(
        GetReviewByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var reviewId = ReviewId.Create(request.ReviewId);
        var review = await reviewRepository.GetReviewByIdAsync(reviewId, cancellationToken);
        if (review == null)
        {
            return Result<ReviewDetailDto>.Failure(ReviewError.ReviewNotFound());
        }
        return Result<ReviewDetailDto>.Success(
            new ReviewDetailDto(
                review.Id.Value,
                review.ProductId.Value,
                review.UserId,
                review.Rating,
                review.Content,
                review.CreatedAt
            )
        );
    }
}
