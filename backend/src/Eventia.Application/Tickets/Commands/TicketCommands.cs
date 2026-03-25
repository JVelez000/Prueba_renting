using Eventia.Application.DTOs;
using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Domain.ValueObjects;
using MediatR;

namespace Eventia.Application.Tickets.Commands;

// --- Create Ticket ---
public record CreateTicketCommand(string Title, string Description, Guid EventId, Guid CreatedById) : IRequest<TicketDto>;

public class CreateTicketCommandHandler(ITicketRepository ticketRepo, IEventRepository eventRepo, IUnitOfWork uow)
    : IRequestHandler<CreateTicketCommand, TicketDto>
{
    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken ct)
    {
        var ev = await eventRepo.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException($"Event {request.EventId} not found.");

        var ticket = Ticket.Create(request.Title, request.Description, request.EventId, request.CreatedById);
        await ticketRepo.AddAsync(ticket, ct);
        await uow.SaveChangesAsync(ct);

        return new TicketDto(
            ticket.Id, ticket.Title, ticket.Description,
            ticket.Status.ToString(), ticket.EventId, ev.Name,
            ticket.CreatedById, null, ticket.AssignedToId, null,
            ticket.CreatedAt, ticket.UpdatedAt);
    }
}

// --- Update Ticket ---
public record UpdateTicketCommand(Guid TicketId, string Title, string Description) : IRequest<TicketDto>;

public class UpdateTicketCommandHandler(ITicketRepository ticketRepo, IUnitOfWork uow)
    : IRequestHandler<UpdateTicketCommand, TicketDto>
{
    public async Task<TicketDto> Handle(UpdateTicketCommand request, CancellationToken ct)
    {
        var ticket = await ticketRepo.GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException("Ticket not found.");

        ticket.Update(request.Title, request.Description);
        ticketRepo.Update(ticket);
        await uow.SaveChangesAsync(ct);

        return new TicketDto(
            ticket.Id, ticket.Title, ticket.Description,
            ticket.Status.ToString(), ticket.EventId, ticket.Event?.Name,
            ticket.CreatedById, ticket.CreatedBy?.Name,
            ticket.AssignedToId, ticket.AssignedTo?.Name,
            ticket.CreatedAt, ticket.UpdatedAt);
    }
}

// --- Assign Ticket ---
public record AssignTicketCommand(Guid TicketId, Guid AssignedToId) : IRequest;

public class AssignTicketCommandHandler(ITicketRepository ticketRepo, IUserRepository userRepo, IUnitOfWork uow)
    : IRequestHandler<AssignTicketCommand>
{
    public async Task Handle(AssignTicketCommand request, CancellationToken ct)
    {
        var ticket = await ticketRepo.GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException("Ticket not found.");

        var user = await userRepo.GetByIdAsync(request.AssignedToId, ct)
            ?? throw new KeyNotFoundException("User to assign not found.");

        ticket.AssignTo(request.AssignedToId);
        ticketRepo.Update(ticket);
        await uow.SaveChangesAsync(ct);
    }
}

// --- Change Status ---
public record ChangeTicketStatusCommand(Guid TicketId, string Status, Guid ChangedById, string? Notes) : IRequest;

public class ChangeTicketStatusCommandHandler(ITicketRepository ticketRepo, IUnitOfWork uow)
    : IRequestHandler<ChangeTicketStatusCommand>
{
    public async Task Handle(ChangeTicketStatusCommand request, CancellationToken ct)
    {
        var ticket = await ticketRepo.GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException("Ticket not found.");

        if (!Enum.TryParse<TicketStatus>(request.Status, true, out var newStatus))
            throw new ArgumentException($"Invalid status: {request.Status}");

        ticket.ChangeStatus(newStatus, request.ChangedById, request.Notes);
        ticketRepo.Update(ticket);
        await uow.SaveChangesAsync(ct);
    }
}

// --- Delete Ticket ---
public record DeleteTicketCommand(Guid TicketId) : IRequest;

public class DeleteTicketCommandHandler(ITicketRepository ticketRepo, IUnitOfWork uow)
    : IRequestHandler<DeleteTicketCommand>
{
    public async Task Handle(DeleteTicketCommand request, CancellationToken ct)
    {
        var ticket = await ticketRepo.GetByIdAsync(request.TicketId, ct)
            ?? throw new KeyNotFoundException("Ticket not found.");

        ticketRepo.Delete(ticket);
        await uow.SaveChangesAsync(ct);
    }
}
