using TaskPro.Domain.Entities;

namespace TaskPro.Domain.Interfaces.Repositories
{
    public interface ICommentsRepository
    {
        Task<Comment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Comment>> GetByTaskAsync(Guid taskId, CancellationToken ct = default);
        Task AddAsync(Comment comment, CancellationToken ct = default);
        Task UpdateAsync(Comment comment, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
