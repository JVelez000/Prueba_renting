using Eventia.Domain.ValueObjects;

namespace Eventia.Application.DTOs;

// Tickets
public record TicketDto(Guid Id, string Title, string Description, string Status, Guid EventId, string? EventName, Guid CreatedById, string? CreatedByName, Guid? AssignedToId, string? AssignedToName, DateTime CreatedAt, DateTime UpdatedAt);
public record CreateTicketRequest(string Title, string Description, Guid EventId);
public record UpdateTicketRequest(string Title, string Description);
public record AssignTicketRequest(Guid UserId);
public record ChangeStatusRequest(string Status, string? Notes);
public record TicketHistoryDto(Guid Id, string OldStatus, string NewStatus, string ChangedBy, string? Notes, DateTime ChangedAt);
public record TicketDetailDto(TicketDto Ticket, IEnumerable<TicketHistoryDto> History);

// Users
public record UserDto(Guid Id, string Name, string Email, string Role, bool IsActive, DateTime CreatedAt);
public record CreateUserRequest(string Name, string Email, string Password, string? Role);
public record UpdateUserRequest(string Name);
public record UpdateUserRoleRequest(string Role);

// Auth
public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string Name, string Email, string Role, Guid UserId);

// Events
public record EventDto(Guid Id, string Name, string Description, DateTime EventDate, string Location, Guid CreatedById, DateTime CreatedAt);
public record CreateEventRequest(string Name, string Description, DateTime EventDate, string Location);
public record UpdateEventRequest(string Name, string Description, DateTime EventDate, string Location);
