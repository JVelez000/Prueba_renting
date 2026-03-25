using Eventia.Domain.Common;
using Eventia.Domain.ValueObjects;

namespace Eventia.Domain.Entities;

public class TicketHistory : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid ChangedById { get; private set; }
    public TicketStatus OldStatus { get; private set; }
    public TicketStatus NewStatus { get; private set; }
    public string? Notes { get; private set; }

    public Ticket? Ticket { get; private set; }
    public User? ChangedBy { get; private set; }

    protected TicketHistory() { }

    public static TicketHistory Create(Guid ticketId, Guid changedById, TicketStatus oldStatus, TicketStatus newStatus, string? notes)
    {
        return new TicketHistory
        {
            TicketId = ticketId,
            ChangedById = changedById,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Notes = notes
        };
    }
}
