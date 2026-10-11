using MarketPlace.Application.Features.Reviews.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Command.Update;

public record class UpdateReviewCommand
(
    Guid ReviewId,
    string Content,
    double Rating
) : IRequest<Result<UpdateReviewDto>>;
