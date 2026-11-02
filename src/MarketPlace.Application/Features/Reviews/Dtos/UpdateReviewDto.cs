namespace MarketPlace.Application.Features.Reviews.Dtos;

public record class UpdateReviewDto
(
    Guid ReviewId,
    string Content,
    decimal Rating
);
