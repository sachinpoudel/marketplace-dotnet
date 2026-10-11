using MarketPlace.Domain.Common.ResultPattern;
using MediatR;

namespace MarketPlace.Application.Features.Reviews.Command.Delete;

public record class DeleteReviewCommand
(
    Guid ReviewId
) : IRequest<Result<bool>>;