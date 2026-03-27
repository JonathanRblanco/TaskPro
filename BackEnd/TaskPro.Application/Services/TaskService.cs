using AutoMapper;
using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Tasks;
using TaskPro.Application.Interfaces;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Application.Services
{
    public class TaskService(IUnitOfWork uow, IMapper mapper) : ITaskService
    {
        public async Task<Result<IEnumerable<TaskItemDTO>>> GetByProjectAsync(
            Guid projectId, Guid requestingUserId)
        {
            var member = await uow.Projects.GetMemberAsync(projectId, requestingUserId);
            if (member is null)
                return Result<IEnumerable<TaskItemDTO>>.Failure("You are not a member of this project.");

            var tasks = await uow.Tasks.GetByProjectAsync(projectId);
            return Result<IEnumerable<TaskItemDTO>>.Success(
                mapper.Map<IEnumerable<TaskItemDTO>>(tasks));
        }

        public async Task<Result<TaskItemDTO>> GetByIdAsync(Guid id, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
            if (task is null)
                return Result<TaskItemDTO>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null)
                return Result<TaskItemDTO>.Failure("You are not a member of this project.");

            return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(task));
        }

        public async Task<Result<TaskItemDTO>> CreateAsync(CreateTaskItemDTO DTO, Guid requestingUserId)
        {
            var member = await uow.Projects.GetMemberAsync(DTO.ProjectId, requestingUserId);
            if (member is null)
                return Result<TaskItemDTO>.Failure("You are not a member of this project.");

            var task = TaskItem.Create(
                DTO.Title, DTO.ProjectId, DTO.Description, DTO.DueDate);

            await uow.Tasks.AddAsync(task);
            await uow.CommitAsync();

            var created = await uow.Tasks.GetByIdWithDetailsAsync(task.Id);
            return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(created));
        }

        public async Task<Result<TaskItemDTO>> UpdateAsync(
            Guid id, UpdateTaskItemDTO DTO, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
            if (task is null)
                return Result<TaskItemDTO>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null)
                return Result<TaskItemDTO>.Failure("You are not a member of this project.");

            if (member.Role == ProjectRole.Contributor && task.AssignedToMemberId != member.Id)
                return Result<TaskItemDTO>.Failure("Contributors can only edit their own tasks.");

            task.UpdateDetails(DTO.Title, DTO.Description, DTO.DueDate);
            uow.Tasks.Update(task);
            await uow.CommitAsync();

            return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(task));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
            if (task is null)
                return Result.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null || member.Role == ProjectRole.Contributor)
                return Result.Failure("You don't have permission to delete this task.");

            uow.Tasks.Delete(task);
            await uow.CommitAsync();

            return Result.Success();
        }

        public async Task<Result<TaskItemDTO>> ChangeStatusAsync(
            Guid id, string status, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
            if (task is null)
                return Result<TaskItemDTO>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null)
                return Result<TaskItemDTO>.Failure("You are not a member of this project.");

            if (!Enum.TryParse<TaskItemStatus>(status, out var newStatus))
                return Result<TaskItemDTO>.Failure("Invalid status value.");

            try
            {
                task.ChangeStatus(newStatus);
            }
            catch (DomainException ex)
            {
                return Result<TaskItemDTO>.Failure(ex.Message);
            }

            uow.Tasks.Update(task);
            await uow.CommitAsync();

            return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(task));
        }

        public async Task<Result<TaskItemDTO>> AssignAsync(Guid id, Guid userId, Guid requestingUserId)
        {
            try
            {
                var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
                if (task is null)
                    return Result<TaskItemDTO>.Failure("Task not found.");

                var requester = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
                if (requester is null || requester.Role == ProjectRole.Contributor)
                    return Result<TaskItemDTO>.Failure("You don't have permission to assign tasks.");

                var assignee = await uow.Projects.GetMemberAsync(task.ProjectId, userId);
                if (assignee is null)
                    return Result<TaskItemDTO>.Failure("Member does not belong to this project.");

                task.AssignTo(assignee.Id);
                uow.Tasks.Update(task);
                await uow.CommitAsync();

                var updated = await uow.Tasks.GetByIdWithDetailsAsync(id);
                return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(updated));
            }
            catch (Exception ex)
            {
                return Result<TaskItemDTO>.Failure(ex.Message);
            }
        }

        public async Task<Result<TaskItemDTO>> UnassignAsync(Guid id, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdWithDetailsAsync(id);
            if (task is null)
                return Result<TaskItemDTO>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null || member.Role == ProjectRole.Contributor)
                return Result<TaskItemDTO>.Failure("You don't have permission to unassign tasks.");

            task.Unassign();
            uow.Tasks.Update(task);
            await uow.CommitAsync();

            return Result<TaskItemDTO>.Success(mapper.Map<TaskItemDTO>(task));
        }

        public async Task<Result<IEnumerable<TaskItemDTO>>> GetByUserAsync(Guid requestingUserId)
        {
            var tasks = await uow.Tasks.GetByUserAsync(requestingUserId);
            return Result<IEnumerable<TaskItemDTO>>.Success(mapper.Map<IEnumerable<TaskItemDTO>>(tasks));
        }
    }
}
