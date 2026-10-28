using MarketPlace.Application.Features.Payments.Command.Create;
using MarketPlace.Application.Features.Payments.Command.Pay;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController (IMediator mediator): ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePaymentCommand request, CancellationToken cancellationToken
        )
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result);
        }
        [HttpPost("pay")]
        public async Task<IActionResult> Pay(
            PayPaymentCommand request, CancellationToken cancellationToken
        )
        {
            var result = await mediator.Send(request, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result);
        }
    }
}
