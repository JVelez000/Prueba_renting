using System.Security.Claims;
using Eventia.Application.DTOs;
using Eventia.Application.Events.Commands;
using Eventia.Application.Events.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventia.API.Controllers;

[ApiController]
[Route("api/events")]
[Authorize]
public class EventsController(IMediator mediator) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not authenticated."));

    /// <summary>Get all events.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetAll()
        => Ok(await mediator.Send(new GetEventsQuery()));

    /// <summary>Get event by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetEventByIdQuery(id)));

    /// <summary>Create a new event (Admin/Supervisor only).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<EventDto>> Create([FromBody] CreateEventRequest request)
    {
        var ev = await mediator.Send(
            new CreateEventCommand(request.Name, request.Description, request.EventDate, request.Location, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id = ev.Id }, ev);
    }

    /// <summary>Update an event (Admin/Supervisor only).</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<ActionResult<EventDto>> Update(Guid id, [FromBody] UpdateEventRequest request)
    {
        var ev = await mediator.Send(
            new UpdateEventCommand(id, request.Name, request.Description, request.EventDate, request.Location));
        return Ok(ev);
    }

    /// <summary>Delete an event (Admin only).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteEventCommand(id));
        return NoContent();
    }
}
