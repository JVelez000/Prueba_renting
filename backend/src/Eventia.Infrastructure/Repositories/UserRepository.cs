using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Eventia.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await db.Users.FindAsync([id], ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        => await db.Users.OrderBy(u => u.Name).ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await db.Users.AddAsync(user, ct);

    public void Update(User user)
        => db.Users.Update(user);
}
