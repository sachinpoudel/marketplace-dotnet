using MarketPlace.Application.Features.Payments.Command.Create;
using MarketPlace.Application.Features.Payments.Command.Fail;
using MarketPlace.Application.Features.Payments.Command.Pay;
using MarketPlace.Application.Features.Payments.Command.Refund;
using MarketPlace.Domain.Payments.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreatePaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsFailure ? Problem(result.Error.Message) : Ok(result);
    }

    [HttpPost("pay")]
    public async Task<IActionResult> Pay(
        PayPaymentCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsFailure ? Problem(result.Error.Message) : Ok(result);
    }

[HttpGet("callback")]
[AllowAnonymous]
public async Task<IActionResult> KhaltiCallback(
    [FromQuery] string pidx,
    [FromQuery] string status,
    [FromQuery(Name = "purchase_order_id")] Guid orderId,
    CancellationToken cancellationToken)
{
    if (!string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
    {
        await mediator.Send(new FailPaymentCommand(orderId), cancellationToken);
        return BadRequest(new 
        { 
            Message = $"Payment was not completed (Status: {status}). You can retry payment for this order.", 
            OrderId = orderId 
        });
    }

    var result = await mediator.Send(new PayPaymentCommand(orderId,Domain.Payments.Enums.PaymentMethod.Khalti), cancellationToken);

    if (result.IsFailure)
    {
        return BadRequest(result.Error);
    }

    return Ok(new 
    { 
        Message = "Payment verified and completed!", 
        Data = result.Value 
    });
}

[HttpPost("refund")]
public async Task<IActionResult> Refund(
    RefundPaymentCommand request,
    CancellationToken cancellationToken
)
{
    var result = await mediator.Send(request, cancellationToken);
    return result.IsFailure ? Problem(result.Error.Message) : Ok(result);
}
}
