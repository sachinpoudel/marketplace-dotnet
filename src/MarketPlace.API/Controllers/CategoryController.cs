using MarketPlace.Application.Features.Categories.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
