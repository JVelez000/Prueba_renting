using System.Security.Claims;
using Eventia.Application.DTOs;
using Eventia.Application.Tickets.Commands;
using Eventia.Application.Tickets.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventia.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController(IMediator mediator) : ControllerBase
{
    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User not authenticated."));

    private bool IsAdminOrSupervisor =>
        User.IsInRole("Admin") || User.IsInRole("Supervisor");

    /// <summary>Get tickets. Admins/Supervisors see all; Agents see their own.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll()
    {
        var userId = IsAdminOrSupervisor ? (Guid?)null : CurrentUserId;
        return Ok(await mediator.Send(new GetTicketsQuery(userId)));
    }

    /// <summary>Get ticket by ID including history.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailDto>> GetById(Guid id)
        => Ok(await mediator.Send(new GetTicketByIdQuery(id)));

    /// <summary>Create a new ticket.</summary>
    [HttpPost]
    public async Task<ActionResult<TicketDto>> Create([FromBody] CreateTicketRequest request)
    {
        var ticket = await mediator.Send(
            new CreateTicketCommand(request.Title, request.Description, request.EventId, CurrentUserId));
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    /// <summary>Update ticket title and description.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TicketDto>> Update(Guid id, [FromBody] UpdateTicketRequest request)
    {
        var ticket = await mediator.Send(new UpdateTicketCommand(id, request.Title, request.Description));
        return Ok(ticket);
    }

    /// <summary>Assign ticket to a user (Admin/Supervisor only).</summary>
    [HttpPut("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTicketRequest request)
    {
        await mediator.Send(new AssignTicketCommand(id, request.UserId));
        return NoContent();
    }

    /// <summary>Change ticket status.</summary>
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusRequest request)
    {
        await mediator.Send(new ChangeTicketStatusCommand(id, request.Status, CurrentUserId, request.Notes));
        return NoContent();
    }

    /// <summary>Delete a ticket (Admin/Supervisor only).</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeleteTicketCommand(id));
        return NoContent();
    }
}
