namespace Eventia.Application.Dashboard.Queries;

public record DashboardStatsDto(
    int OpenTickets,
    int InProgressTickets,
    int ClosedTickets,
    int ActiveEvents
);
