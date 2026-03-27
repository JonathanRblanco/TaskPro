using TaskPro.Domain.Entities;

namespace TaskPro.Domain.Interfaces.Repositories
{
    public interface IProjectsRepository
    {
        Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Project>> GetByUserAsync(Guid userId, CancellationToken ct = default);
        Task<ProjectMember?> GetMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
        Task AddAsync(Project project, CancellationToken ct = default);
        void Update(Project project);
        void Delete(Project project);
        Task<IEnumerable<Project>> GetAllAsync(CancellationToken ct = default);
    }
}
