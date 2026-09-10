using MarketPlace.Application.Features.Auth.Command.Login;
using MarketPlace.Application.Features.Auth.Command.RefreshToken;
using MarketPlace.Application.Features.Auth.Command.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.API.Controllers;

[ApiController]
[Route("api/[controller]")]


public class AuthController(ILogger<AuthController> _logger, IMediator _mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterUserCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.IsSuccess)
        {
            _logger.LogError("User registration failed");
            return BadRequest(result.Error);
        }
        return Ok();
    }
    
public async Task<IActionResult> LoginUser(LoginUserCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.IsSuccess)
        {
            _logger.LogError("User login failed");
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand request)
    {
        var result = await _mediator.Send(request);
        if (!result.IsSuccess)
        {
            _logger.LogError("Token refresh failed");
            return BadRequest(result.Error);
        }
        return Ok(result.Value);
    }

}
