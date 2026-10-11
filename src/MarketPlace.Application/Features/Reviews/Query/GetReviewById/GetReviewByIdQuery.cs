using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Query.GetReviewById;

public record class GetReviewByIdQuery
(
    Guid ReviewId
) : IRequest<Result<ReviewDetailDto>>;
