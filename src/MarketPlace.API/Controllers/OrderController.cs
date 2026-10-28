using MarketPlace.Application.Features.Orders.Command.Cancel;
using MarketPlace.Application.Features.Orders.Command.Confirm;
using MarketPlace.Application.Features.Orders.Command.Create;
using MarketPlace.Application.Features.Orders.Command.Deliver;
using MarketPlace.Application.Features.Orders.Command.Process;
using MarketPlace.Application.Features.Orders.Command.Ship;
using MarketPlace.Application.Features.Orders.Dtos;
using MarketPlace.Application.Features.Orders.Queries.GetMyOrders;
using MarketPlace.Application.Features.Orders.Queries.GetOrderById;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            CreateOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            var result = await mediator.Send(command, cancellationToken);

            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }

        [HttpPost("cancel/{orderId:guid}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new CancelOrderCommand(orderId);
            var result = await mediator.Send(command,cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }

        [HttpPost("confirm/{orderId:guid}")]
        public async Task<IActionResult> ConfirmOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new ConfirmOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);

            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }

        [HttpPost("delivery/{orderId:guid}")]
        public async Task<IActionResult> DeliverOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new DeliverOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
            // return await result.
        }

        [HttpPost("process/{orderId:guid}")]
        public async Task<IActionResult> ProcessOrder([FromRoute] Guid orderId, CancellationToken cancellationToken)
        {
            var command = new ProcessOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }

        [HttpPost("ship/{orderId:guid}")]
        public async Task<IActionResult> ShipOrder(
            [FromRoute] Guid orderId  ,       CancellationToken cancellationToken
        )
        {
            var command = new ShipOrderCommand(orderId);
            var updatedResult = await mediator.Send(command, cancellationToken);
            return updatedResult.IsFailure ? Problem(updatedResult.Error.Message) : Ok(updatedResult.Value);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetMyOrdersQuery(pageNumber, pageSize);
            var result = await mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpGet("orders/{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetOrderByIdQuery(orderId);
            var result = await mediator.Send(query, cancellationToken);
            return result.IsFailure ? Problem(result.Error.Message) : Ok(result.Value);
        }
    }
}
