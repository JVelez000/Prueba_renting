using Eventia.Application.DTOs;
using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Tickets.Queries;

// --- Get All Tickets ---
public record GetTicketsQuery(Guid? UserId = null) : IRequest<IEnumerable<TicketDto>>;

public class GetTicketsQueryHandler(ITicketRepository ticketRepo) : IRequestHandler<GetTicketsQuery, IEnumerable<TicketDto>>
{
    public async Task<IEnumerable<TicketDto>> Handle(GetTicketsQuery request, CancellationToken ct)
    {
        var tickets = request.UserId.HasValue
            ? await ticketRepo.GetByUserAsync(request.UserId.Value, ct)
            : await ticketRepo.GetAllAsync(ct);

        return tickets.Select(t => new TicketDto(
            t.Id, t.Title, t.Description,
            t.Status.ToString(), t.EventId, t.Event?.Name,
            t.CreatedById, t.CreatedBy?.Name,
            t.AssignedToId, t.AssignedTo?.Name,
            t.CreatedAt, t.UpdatedAt));
    }
}

// --- Get Ticket By Id ---
public record GetTicketByIdQuery(Guid TicketId) : IRequest<TicketDetailDto>;

public class GetTicketByIdQueryHandler(ITicketRepository ticketRepo) : IRequestHandler<GetTicketByIdQuery, TicketDetailDto>
{
    public async Task<TicketDetailDto> Handle(GetTicketByIdQuery request, CancellationToken ct)
    {
        var ticket = await ticketRepo.GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException("Ticket not found.");

        var ticketDto = new TicketDto(
            ticket.Id, ticket.Title, ticket.Description,
            ticket.Status.ToString(), ticket.EventId, ticket.Event?.Name,
            ticket.CreatedById, ticket.CreatedBy?.Name,
            ticket.AssignedToId, ticket.AssignedTo?.Name,
            ticket.CreatedAt, ticket.UpdatedAt);

        var history = ticket.History.OrderByDescending(h => h.CreatedAt).Select(h => new TicketHistoryDto(
            h.Id, h.OldStatus.ToString(), h.NewStatus.ToString(),
            h.ChangedById.ToString(), h.Notes, h.CreatedAt));

        return new TicketDetailDto(ticketDto, history);
    }
}
