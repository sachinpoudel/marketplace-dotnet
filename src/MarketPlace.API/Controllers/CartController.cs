using MarketPlace.Application.Features.Carts.Commands.AddItem;
using MarketPlace.Application.Features.Carts.Commands.ChangeQuantity;
using MarketPlace.Application.Features.Carts.Commands.RemoveItem;
using MarketPlace.Application.Features.Carts.Dtos;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        AddCartItemCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator
            .Send(command, cancellationToken)
            .MatchAsync<CartDetailDto, IActionResult>( // Explicitly stating TResult here also works
                success => Ok(success),
                error => Problem(error.Message)
            );
        return result;
    }

    [HttpPut("change-quantity/{cartId:guid}")]
    public async Task<IActionResult> ChangeQuantity(
        [FromRoute] Guid cartId,
        [FromBody] ChangeQuantityCommand request,
        CancellationToken cancellationToken
    )
    {
          var command = new ChangeQuantityCommand(
        cartId,
        request.ProductId,
        request.Quantity
    );

        var result = await mediator
            .Send(command, cancellationToken)
            .MatchAsync<CartDetailDto, IActionResult>(
                onSuccess => Ok(onSuccess),
                onFailure => Problem(onFailure.Message)
            );

        return result;
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(
   
        [FromRoute] Guid cartItemId,
        CancellationToken cancellationToken
    )
    {
        var command = new RemoveItemCommand(cartItemId);
        var result = await mediator
            .Send(command, cancellationToken)
            .MatchAsync<CartDetailDto, IActionResult>(
                onSuccess => Ok(onSuccess),
                onFailure => Problem(onFailure.Message)
            );

        return result;
    }
}
