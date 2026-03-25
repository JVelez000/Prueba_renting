using Eventia.Domain.Common;
using Eventia.Domain.Events;
using Eventia.Domain.ValueObjects;

namespace Eventia.Domain.Entities;

public class Ticket : BaseEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public TicketStatus Status { get; private set; } = TicketStatus.Open;
    public Guid CreatedById { get; private set; }
    public Guid? AssignedToId { get; private set; }
    public Guid EventId { get; private set; }

    public User? CreatedBy { get; private set; }
    public User? AssignedTo { get; private set; }
    public Event? Event { get; private set; }
    public ICollection<TicketHistory> History { get; private set; } = new List<TicketHistory>();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();

    protected Ticket() { }

    public static Ticket Create(string title, string description, Guid eventId, Guid createdById)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        var ticket = new Ticket
        {
            Title = title,
            Description = description,
            EventId = eventId,
            CreatedById = createdById,
            Status = TicketStatus.Open
        };
        ticket._domainEvents.Add(new TicketCreatedEvent(ticket.Id, ticket.Title, createdById));
        return ticket;
    }

    public void AssignTo(Guid userId)
    {
        AssignedToId = userId;
        Touch();
    }

    public void ChangeStatus(TicketStatus newStatus, Guid changedById, string? notes = null)
    {
        var oldStatus = Status;
        Status = newStatus;
        Touch();

        History.Add(TicketHistory.Create(Id, changedById, oldStatus, newStatus, notes));
        _domainEvents.Add(new TicketStatusChangedEvent(Id, oldStatus, newStatus, changedById));
    }

    public void Update(string title, string description)
    {
        Title = title;
        Description = description;
        Touch();
    }
}
