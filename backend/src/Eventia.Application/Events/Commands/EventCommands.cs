using Eventia.Application.DTOs;
using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Events.Commands;

// --- Create Event ---
public record CreateEventCommand(string Name, string Description, DateTime EventDate, string Location, Guid CreatedById) : IRequest<EventDto>;

public class CreateEventCommandHandler(IEventRepository eventRepo, IUnitOfWork uow)
    : IRequestHandler<CreateEventCommand, EventDto>
{
    public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken ct)
    {
        var ev = Event.Create(request.Name, request.Description, request.EventDate, request.Location, request.CreatedById);
        await eventRepo.AddAsync(ev, ct);
        await uow.SaveChangesAsync(ct);
        return new EventDto(ev.Id, ev.Name, ev.Description, ev.EventDate, ev.Location, ev.CreatedById, ev.CreatedAt);
    }
}

// --- Update Event ---
public record UpdateEventCommand(Guid EventId, string Name, string Description, DateTime EventDate, string Location) : IRequest<EventDto>;

public class UpdateEventCommandHandler(IEventRepository eventRepo, IUnitOfWork uow)
    : IRequestHandler<UpdateEventCommand, EventDto>
{
    public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken ct)
    {
        var ev = await eventRepo.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException("Event not found.");

        ev.Update(request.Name, request.Description, request.EventDate, request.Location);
        eventRepo.Update(ev);
        await uow.SaveChangesAsync(ct);
        return new EventDto(ev.Id, ev.Name, ev.Description, ev.EventDate, ev.Location, ev.CreatedById, ev.CreatedAt);
    }
}

// --- Delete Event ---
public record DeleteEventCommand(Guid EventId) : IRequest;

public class DeleteEventCommandHandler(IEventRepository eventRepo, IUnitOfWork uow)
    : IRequestHandler<DeleteEventCommand>
{
    public async Task Handle(DeleteEventCommand request, CancellationToken ct)
    {
        var ev = await eventRepo.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException("Event not found.");

        eventRepo.Delete(ev);
        await uow.SaveChangesAsync(ct);
    }
}
