namespace Eventia.Domain.Events;

public record TicketCreatedEvent(Guid TicketId, string Title, Guid CreatedById) : IDomainEvent;
