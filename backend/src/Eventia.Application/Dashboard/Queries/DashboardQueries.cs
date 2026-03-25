using Eventia.Domain.Interfaces;
using MediatR;

namespace Eventia.Application.Dashboard.Queries;

public record GetDashboardStatsQuery() : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(IDashboardRepository dashboardRepository) 
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var open = await dashboardRepository.GetOpenTicketsCountAsync(cancellationToken);
        var inProgress = await dashboardRepository.GetInProgressTicketsCountAsync(cancellationToken);
        var closed = await dashboardRepository.GetClosedTicketsCountAsync(cancellationToken);
        var activeEvents = await dashboardRepository.GetActiveEventsCountAsync(cancellationToken);

        return new DashboardStatsDto(open, inProgress, closed, activeEvents);
    }
}
