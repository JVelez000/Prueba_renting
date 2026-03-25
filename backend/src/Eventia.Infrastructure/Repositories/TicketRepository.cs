using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eventia.Infrastructure.Repositories;

public class TicketRepository(AppDbContext db) : ITicketRepository
{
    private IQueryable<Ticket> TicketsWithIncludes =>
        db.Tickets
          .Include(t => t.CreatedBy)
          .Include(t => t.AssignedTo)
          .Include(t => t.Event)
          .Include(t => t.History);

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await TicketsWithIncludes.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken ct = default)
        => await TicketsWithIncludes.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);

    public async Task<IEnumerable<Ticket>> GetByUserAsync(Guid userId, CancellationToken ct = default)
        => await TicketsWithIncludes
               .Where(t => t.CreatedById == userId || t.AssignedToId == userId)
               .OrderByDescending(t => t.CreatedAt)
               .ToListAsync(ct);

    public async Task AddAsync(Ticket ticket, CancellationToken ct = default)
        => await db.Tickets.AddAsync(ticket, ct);

    public void Update(Ticket ticket)
        => db.Tickets.Update(ticket);

    public void Delete(Ticket ticket)
        => db.Tickets.Remove(ticket);
}
