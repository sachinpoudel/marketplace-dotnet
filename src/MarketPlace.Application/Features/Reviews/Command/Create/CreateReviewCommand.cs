using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Command.Create;


public record CreateReviewcommand(

     string Content,
     Guid UserId,
     double Rating,
     Guid ProductId
): IRequest<Result<ReviewDetailDto>>;