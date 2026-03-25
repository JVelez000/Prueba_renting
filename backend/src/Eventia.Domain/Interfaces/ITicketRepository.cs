using Eventia.Domain.Entities;

namespace Eventia.Domain.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Ticket>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Ticket ticket, CancellationToken ct = default);
    void Update(Ticket ticket);
    void Delete(Ticket ticket);
}
