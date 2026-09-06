using MarketPlace.Application.Features.Vendors.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendorController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
