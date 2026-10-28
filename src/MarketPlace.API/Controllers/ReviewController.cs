using MarketPlace.Application.Features.Reviews.Command.Create;
using MarketPlace.Application.Features.Reviews.Query.GetReviewById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReviewcommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }

        [HttpGet("{reviewId:guid}")]
        public async Task<IActionResult> GetReview(
            [FromRoute] Guid reviewId,
            CancellationToken cancellationToken
        )
        {
            var query = new GetReviewByIdQuery(reviewId);
            var result = await mediator.Send(query, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }
    }
}
