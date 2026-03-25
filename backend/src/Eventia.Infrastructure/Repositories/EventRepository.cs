using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eventia.Infrastructure.Repositories;

public class EventRepository(AppDbContext db) : IEventRepository
{
    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Events
               .Include(e => e.CreatedBy)
               .Include(e => e.Tickets)
               .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken ct = default)
        => await db.Events
               .Include(e => e.CreatedBy)
               .OrderByDescending(e => e.EventDate)
               .ToListAsync(ct);

    public async Task AddAsync(Event ev, CancellationToken ct = default)
        => await db.Events.AddAsync(ev, ct);

    public void Update(Event ev)
        => db.Events.Update(ev);

    public void Delete(Event ev)
        => db.Events.Remove(ev);
}
