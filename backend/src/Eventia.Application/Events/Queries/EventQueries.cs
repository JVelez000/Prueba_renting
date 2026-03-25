using Eventia.Application.DTOs;
using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Events.Queries;

// --- Get All Events ---
public record GetEventsQuery : IRequest<IEnumerable<EventDto>>;

public class GetEventsQueryHandler(IEventRepository eventRepo) : IRequestHandler<GetEventsQuery, IEnumerable<EventDto>>
{
    public async Task<IEnumerable<EventDto>> Handle(GetEventsQuery request, CancellationToken ct)
    {
        var events = await eventRepo.GetAllAsync(ct);
        return events.Select(e => new EventDto(e.Id, e.Name, e.Description, e.EventDate, e.Location, e.CreatedById, e.CreatedAt));
    }
}

// --- Get Event By Id ---
public record GetEventByIdQuery(Guid EventId) : IRequest<EventDto>;

public class GetEventByIdQueryHandler(IEventRepository eventRepo) : IRequestHandler<GetEventByIdQuery, EventDto>
{
    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken ct)
    {
        var ev = await eventRepo.GetByIdAsync(request.EventId, ct)
            ?? throw new KeyNotFoundException("Event not found.");

        return new EventDto(ev.Id, ev.Name, ev.Description, ev.EventDate, ev.Location, ev.CreatedById, ev.CreatedAt);
    }
}
