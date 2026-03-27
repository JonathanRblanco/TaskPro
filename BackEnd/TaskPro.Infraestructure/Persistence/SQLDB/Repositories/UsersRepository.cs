using Microsoft.EntityFrameworkCore;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Infraestructure.Persistence.SQLDB.Repositories
{
    public class UsersRepository(ApplicationDbContext context) : IUsersRepository
    {
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) => await context.Users.ToListAsync(ct);
        public async Task<IEnumerable<User>> GetByRolesAsync(IEnumerable<ApplicationRole> roles, CancellationToken ct = default)
            => await context.Users
                    .Where(u => roles.Contains(u.Role))
                    .ToListAsync(ct);
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await context.Users
                            .Include(u => u.Memberships)
                            .FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await context.Users
                            .FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

        public async Task<bool> ExistsWithEmailAsync(string email, CancellationToken ct = default)
            => await context.Users
                            .AnyAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
            => await context.Users.AddAsync(user, ct);

        public void Update(User user)
            => context.Users.Update(user);

        public void Delete(User user)
            => context.Users.Remove(user);
    }
}
