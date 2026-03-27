using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Projects;

namespace TaskPro.Application.Interfaces
{
    public interface IProjectService
    {
        Task<Result<IEnumerable<ProjectDTO>>> GetByUserAsync(Guid userId);
        Task<Result<ProjectDTO>> GetByIdAsync(Guid id);
        Task<Result<ProjectDTO>> CreateAsync(CreateProjectDTO dto, Guid ownerId);
        Task<Result<ProjectDTO>> UpdateAsync(Guid id, UpdateProjectDTO dto, Guid requestingUserId);
        Task<Result> DeleteAsync(Guid id, Guid requestingUserId);
        Task<Result<ProjectDTO>> CompleteAsync(Guid id, Guid requestingUserId);
        Task<Result<ProjectDTO>> CancelAsync(Guid id, Guid requestingUserId);
        Task<Result<ProjectDTO>> ArchiveAsync(Guid id, Guid requestingUserId);
        Task<Result<ProjectDTO>> AddMemberAsync(Guid projectId, Guid userId, string role, Guid requestingUserId);
        Task<Result> RemoveMemberAsync(Guid projectId, Guid userId, Guid requestingUserId);
        Task<Result<IEnumerable<ProjectMemberDTO>>> GetMembersAsync(Guid projectId, Guid requestingUserId);
        Task<Result<IEnumerable<ProjectDTO>>> GetAllAsync();
    }
}
