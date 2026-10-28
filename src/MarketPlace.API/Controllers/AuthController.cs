using MarketPlace.Application.Features.Auth.Command.Login;
using MarketPlace.Application.Features.Auth.Command.RefreshToken;
using MarketPlace.Application.Features.Auth.Command.Register;
using MarketPlace.Domain.Common.ResultPattern;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> _logger, IMediator _mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogError("User login failed");
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogError("Token refresh failed");
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
