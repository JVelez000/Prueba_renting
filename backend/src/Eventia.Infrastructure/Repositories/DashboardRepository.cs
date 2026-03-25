using Eventia.Domain.Interfaces;
using Eventia.Domain.ValueObjects;
using Eventia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eventia.Infrastructure.Repositories;

public class DashboardRepository(AppDbContext context) : IDashboardRepository
{
    public async Task<int> GetOpenTicketsCountAsync(CancellationToken cancellationToken)
        => await context.Tickets.CountAsync(t => t.Status == TicketStatus.Open, cancellationToken);

    public async Task<int> GetInProgressTicketsCountAsync(CancellationToken cancellationToken)
        => await context.Tickets.CountAsync(t => t.Status == TicketStatus.InProgress, cancellationToken);

    public async Task<int> GetClosedTicketsCountAsync(CancellationToken cancellationToken)
        => await context.Tickets.CountAsync(t => t.Status == TicketStatus.Closed, cancellationToken);

    public async Task<int> GetActiveEventsCountAsync(CancellationToken cancellationToken)
        => await context.Events.CountAsync(e => e.EventDate >= DateTime.UtcNow.Date, cancellationToken);
}
