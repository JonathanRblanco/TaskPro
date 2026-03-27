using TaskPro.Domain.Entities;

namespace TaskPro.Domain.Interfaces.Repositories
{
    public interface ITasksRepository
    {
        Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<TaskItem>> GetByProjectAsync(Guid projectId, CancellationToken ct = default);
        Task<IEnumerable<TaskItem>> GetByMemberAsync(Guid memberId, CancellationToken ct = default);
        Task AddAsync(TaskItem task, CancellationToken ct = default);
        void Update(TaskItem task);
        void Delete(TaskItem task);
        Task<IEnumerable<TaskItem>> GetByUserAsync(Guid requestingUserId);
    }
}
