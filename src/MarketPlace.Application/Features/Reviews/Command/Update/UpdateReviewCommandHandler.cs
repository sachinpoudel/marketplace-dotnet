using System;
using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Auth;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Reviews.ValueObjects;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Command.Update;

public class UpdateReviewCommandHandler(
    ICurrentUser currentUser,
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateReviewCommand, Result<UpdateReviewDto>>
{
    public async Task<Result<UpdateReviewDto>> Handle(
        UpdateReviewCommand request,
        CancellationToken cancellationToken
    )
    {
        var currentUserId = currentUser.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result<UpdateReviewDto>.Failure(UserError.UserNotAuthenticated());
        }
        var review = await reviewRepository.GetReviewByIdAsync(
            ReviewId.Create(request.ReviewId),
            cancellationToken
        );
        if (review == null)
        {
            return Result<UpdateReviewDto>.Failure(ReviewError.ReviewNotFound());
        }
        if (review.UserId != currentUserId)
        {
            return Result<UpdateReviewDto>.Failure(UserError.UserNotAuthorized());
        }
        review.Update(request.Content, request.Rating);
        await reviewRepository.UpdateReviewAsync(review);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result<UpdateReviewDto>.Success(
            new UpdateReviewDto(review.Id.Value, review.Content, review.Rating)
        );
    }
}
