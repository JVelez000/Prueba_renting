using Eventia.Application.DTOs;
using Eventia.Application.Users.Commands;
using Eventia.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventia.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin,Supervisor")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>Get all users.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        => Ok(await mediator.Send(new GetUsersQuery()));

    /// <summary>Get user by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetUserByIdQuery(id)));

    /// <summary>Disable a user (Admin only).</summary>
    [HttpPut("{id:guid}/disable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Disable(Guid id)
    {
        await mediator.Send(new DisableUserCommand(id));
        return NoContent();
    }

    /// <summary>Enable a user (Admin only).</summary>
    [HttpPut("{id:guid}/enable")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Enable(Guid id)
    {
        await mediator.Send(new EnableUserCommand(id));
        return NoContent();
    }

    /// <summary>Update a user's role (Admin only). Valid roles: Agent, Supervisor, Admin.</summary>
    [HttpPut("{id:guid}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> UpdateRole(Guid id, [FromBody] UpdateUserRoleRequest request)
    {
        var result = await mediator.Send(new UpdateUserRoleCommand(id, request.Role));
        return Ok(result);
    }
}
