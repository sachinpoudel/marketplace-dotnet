using System;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Reviews.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Command.Delete;

public class DeleteReviewCommandHandler(
    ICurrentUser currentUser,
    IReviewRepository reviewRepository
) : IRequestHandler<DeleteReviewCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();

        if (currentUserId == null)
        {
            return Result<bool>.Failure(UserError.UserNotAuthenticated());
        }
        var review = await reviewRepository.GetReviewByIdAsync(
            ReviewId.Create(request.ReviewId),
            cancellationToken
        );
        if (review == null)
        {
            return Result<bool>.Failure(ReviewError.ReviewNotFound());
        }
        if (review.UserId != currentUserId)
        {
            return Result<bool>.Failure(UserError.UserNotAuthorized());
        }
        review.Delete();
        await reviewRepository.UpdateReviewAsync(review);
        return Result<bool>.Success(true);
    }
}
