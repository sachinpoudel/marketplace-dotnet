using MarketPlace.API.Handlers;
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
            [FromBody] CreateOrderCommand command,
            CancellationToken cancellationToken
        )
        {
            var result = await mediator
                .Send(command, cancellationToken)
                .MatchAsync<OrderDetailDto, IActionResult>(
                    onSuccess => Ok(onSuccess),
                    onFailure => Problem(onFailure.Message)
                );
            return result;
            // return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
        }

        [HttpPost("cancel/{orderId:guid}")]
        public async Task<IActionResult> CancelOrder(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken
        )
        {
            var command = new CancelOrderCommand(orderId);
            return await mediator
                .Send(command, cancellationToken)
                .MatchAsync<CancelOrderDto, IActionResult>(
                    onSuccess => Ok(onSuccess),
                    onFailure => Problem(onFailure.Message)
                );
            // return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
        }

        [HttpPost("confirm/{orderId:guid}")]
        public async Task<IActionResult> ConfirmOrder(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken
        )
        {
            var command = new ConfirmOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);
            return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
        }

        [HttpPost("delivery/{orderId:guid}")]
        public async Task<IActionResult> DeliverOrder(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken
        )
        {
            var command = new DeliverOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);
            return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
            // return await result.
        }

        [HttpPost("process/{orderId:guid}")]
        public async Task<IActionResult> ProcessOrder(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken
        )
        {
            var command = new ProcessOrderCommand(orderId);
            var result = await mediator.Send(command, cancellationToken);
            return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
        }

        [HttpPost("ship/{orderId:guid}")]
        public async Task<IActionResult> ShipOrder(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken
        )
        {
            var command = new ShipOrderCommand(orderId);
            var updatedResult = await mediator.Send(command, cancellationToken);
            return updatedResult.IsFailure
                ? updatedResult.Error.ToActionResult()
                : Ok(updatedResult.Value);
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

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(
            [FromRoute] Guid orderId,
            CancellationToken cancellationToken = default
        )
        {
            if(orderId == Guid.Empty)
            {
                return BadRequest("Invalid order ID.");
            }
            var query = new GetOrderByIdQuery(orderId);
            Console.WriteLine($"Received request to get order with ID: {orderId}"); // Debugging line
            var result = await mediator.Send(query, cancellationToken);
            return result.IsFailure ? result.Error.ToActionResult() : Ok(result.Value);
        }
    }
}
