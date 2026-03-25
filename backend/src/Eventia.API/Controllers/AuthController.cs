using Eventia.Application.Auth.Commands;
using Eventia.Application.DTOs;
using Eventia.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventia.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>Login and get a JWT token.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(result);
    }

    /// <summary>Register a new user (defaults to Agent role if not specified).</summary>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserRequest request)
    {
        var result = await mediator.Send(
            new CreateUserCommand(request.Name, request.Email, request.Password, request.Role));
        return CreatedAtAction(nameof(Register), result);
    }
}
