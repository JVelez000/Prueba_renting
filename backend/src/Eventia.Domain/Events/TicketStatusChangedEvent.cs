using Eventia.Domain.ValueObjects;

namespace Eventia.Domain.Events;

public record TicketStatusChangedEvent(Guid TicketId, TicketStatus OldStatus, TicketStatus NewStatus, Guid ChangedById) : IDomainEvent;
