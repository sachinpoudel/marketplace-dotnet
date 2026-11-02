namespace MarketPlace.Application.Features.Reviews.Dtos;


public record ReviewDetailDto (
   Guid ReviewId,
   Guid ProductId,
   Guid UserId,
   decimal Rating,
   string Content,
   DateTime CreatedAt
      
);