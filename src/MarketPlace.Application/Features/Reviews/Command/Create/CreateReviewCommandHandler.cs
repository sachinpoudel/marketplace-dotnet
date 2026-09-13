using MaketPlace.Application.Common.Interfaces.UnitOfWork;
using MarketPlace.Application.Common.Interfaces.Repositories;
using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.BaseErrors.Errors;
using MarketPlace.Domain.Common.ResultPattern;
using MarketPlace.Domain.Products.ValueObjects;
using MarketPlace.Domain.Reviews.Entities;
using MarketPlace.Domain.Reviews.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MarketPlace.Application.Features.Reviews.Command.Create;



public class CreateReviewCommandHandler(IUnitOfWork unitOfWork
,
IReviewRepository reviewRepository,
ILogger<CreateReviewCommandHandler> logger
) : IRequestHandler<CreateReviewcommand, Result<ReviewDetailDto>>
{
   

    public async Task<Result<ReviewDetailDto>> Handle(CreateReviewcommand request, CancellationToken cancellationToken)
    {
        var reviewId = ReviewId.Create();
        var productId = ProductId.Create(request.ProductId);

        var review = Review.Create(reviewId,request.UserId, request.Content, request.Rating, productId);

   var reviewResult = await reviewRepository.AddReviewAsync(review);
if(reviewResult is null )
        {
            return Result<ReviewDetailDto>.Failure(ReviewError.ReviewPublishFailed());
        }
   await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Review created successfully.");




        var reviewDetailDto = new ReviewDetailDto(reviewResult.Id.Value, reviewResult.ProductId.Value, reviewResult.UserId, reviewResult.Rating, reviewResult.Content, reviewResult.CreatedAt);
        

        return Result<ReviewDetailDto>.Success(reviewDetailDto);
    }
}