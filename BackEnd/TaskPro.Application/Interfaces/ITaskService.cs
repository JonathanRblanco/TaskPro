using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Tasks;

namespace TaskPro.Application.Interfaces
{
    public interface ITaskService
    {
        Task<Result<IEnumerable<TaskItemDTO>>> GetByProjectAsync(Guid projectId, Guid requestingUserId);
        Task<Result<TaskItemDTO>> GetByIdAsync(Guid id, Guid requestingUserId);
        Task<Result<TaskItemDTO>> CreateAsync(CreateTaskItemDTO dto, Guid requestingUserId);
        Task<Result<TaskItemDTO>> UpdateAsync(Guid id, UpdateTaskItemDTO dto, Guid requestingUserId);
        Task<Result> DeleteAsync(Guid id, Guid requestingUserId);
        Task<Result<TaskItemDTO>> ChangeStatusAsync(Guid id, string status, Guid requestingUserId);
        Task<Result<TaskItemDTO>> AssignAsync(Guid id, Guid userId, Guid requestingUserId);
        Task<Result<TaskItemDTO>> UnassignAsync(Guid id, Guid requestingUserId);
        Task<Result<IEnumerable<TaskItemDTO>>> GetByUserAsync(Guid requestingUserId);
    }
}
