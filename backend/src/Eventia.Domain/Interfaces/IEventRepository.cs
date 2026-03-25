using Eventia.Domain.Entities;

namespace Eventia.Domain.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Event>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(Event ev, CancellationToken ct = default);
    void Update(Event ev);
    void Delete(Event ev);
}
