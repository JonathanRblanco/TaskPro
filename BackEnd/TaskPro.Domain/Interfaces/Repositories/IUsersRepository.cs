using TaskPro.Domain.Entities;
using TaskPro.Domain.Enums;

namespace TaskPro.Domain.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<User>> GetByRolesAsync(IEnumerable<ApplicationRole> roles, CancellationToken ct = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> ExistsWithEmailAsync(string email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        void Update(User user);
        void Delete(User user);
    }
}
