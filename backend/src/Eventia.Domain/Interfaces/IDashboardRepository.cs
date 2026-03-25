namespace Eventia.Domain.Interfaces;

public interface IDashboardRepository
{
    Task<int> GetOpenTicketsCountAsync(CancellationToken cancellationToken);
    Task<int> GetInProgressTicketsCountAsync(CancellationToken cancellationToken);
    Task<int> GetClosedTicketsCountAsync(CancellationToken cancellationToken);
    Task<int> GetActiveEventsCountAsync(CancellationToken cancellationToken);
}
