using AutoMapper;
using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Projects;
using TaskPro.Application.Interfaces;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Application.Services
{
    public class ProjectService(IUnitOfWork uow, IMapper mapper) : IProjectService
    {
        public async Task<Result<IEnumerable<ProjectDTO>>> GetAllAsync()
        {
            var projects = await uow.Projects.GetAllAsync();
            return Result<IEnumerable<ProjectDTO>>.Success(mapper.Map<IEnumerable<ProjectDTO>>(projects));
        }
        public async Task<Result<IEnumerable<ProjectDTO>>> GetByUserAsync(Guid userId)
        {
            var projects = await uow.Projects.GetByUserAsync(userId);
            return Result<IEnumerable<ProjectDTO>>.Success(mapper.Map<IEnumerable<ProjectDTO>>(projects));
        }

        public async Task<Result<ProjectDTO>> GetByIdAsync(Guid id)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(id);
            if (project is null)
                return Result<ProjectDTO>.Failure("Project not found.");

            return Result<ProjectDTO>.Success(mapper.Map<ProjectDTO>(project));
        }

        public async Task<Result<ProjectDTO>> CreateAsync(CreateProjectDTO DTO, Guid ownerId)
        {
            var owner = await uow.Users.GetByIdAsync(ownerId);
            if (owner is null)
                return Result<ProjectDTO>.Failure("Owner user not found.");

            var project = Domain.Entities.Project.Create(DTO.Name, ownerId, DTO.Description);

            project.AddMember(ownerId, ProjectRole.Owner);

            await uow.Projects.AddAsync(project);
            await uow.CommitAsync();

            var created = await uow.Projects.GetByIdWithMembersAsync(project.Id);
            return Result<ProjectDTO>.Success(mapper.Map<ProjectDTO>(created));
        }

        public async Task<Result<ProjectDTO>> UpdateAsync(Guid id, UpdateProjectDTO DTO, Guid requestingUserId)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(id);
            if (project is null)
                return Result<ProjectDTO>.Failure("Project not found.");

            var member = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (member is null || member.Role == ProjectRole.Contributor)
                return Result<ProjectDTO>.Failure("You don't have permission to edit this project.");

            project.UpdateDetails(DTO.Name, DTO.Description);
            uow.Projects.Update(project);
            await uow.CommitAsync();

            return Result<ProjectDTO>.Success(mapper.Map<ProjectDTO>(project));
        }

        public async Task<Result> DeleteAsync(Guid id, Guid requestingUserId)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(id);
            if (project is null)
                return Result.Failure("Project not found.");

            var member = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (member is null || member.Role != ProjectRole.Owner)
                return Result.Failure("Only the project owner can delete this project.");

            uow.Projects.Delete(project);
            await uow.CommitAsync();

            return Result.Success();
        }

        public async Task<Result<ProjectDTO>> CompleteAsync(Guid id, Guid requestingUserId)
            => await ChangeStatusAsync(id, requestingUserId,
                project => project.Complete());

        public async Task<Result<ProjectDTO>> CancelAsync(Guid id, Guid requestingUserId)
            => await ChangeStatusAsync(id, requestingUserId,
                project => project.Cancel());

        public async Task<Result<ProjectDTO>> ArchiveAsync(Guid id, Guid requestingUserId)
            => await ChangeStatusAsync(id, requestingUserId,
                project => project.Archive());

        public async Task<Result<ProjectDTO>> AddMemberAsync(
            Guid projectId, Guid userId, string role, Guid requestingUserId)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(projectId);
            if (project is null)
                return Result<ProjectDTO>.Failure("Project not found.");

            var requester = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (requester is null || requester.Role == ProjectRole.Contributor)
                return Result<ProjectDTO>.Failure("You don't have permission to add members.");

            var userExists = await uow.Users.GetByIdAsync(userId);
            if (userExists is null)
                return Result<ProjectDTO>.Failure("User not found.");

            if (!Enum.TryParse<ProjectRole>(role, out var projectRole))
                return Result<ProjectDTO>.Failure("Invalid project role.");

            project.AddMember(userId, projectRole);
            await uow.CommitAsync();
            var updated = await uow.Projects.GetByIdWithMembersAsync(projectId);
            return Result<ProjectDTO>.Success(mapper.Map<ProjectDTO>(updated));
        }

        public async Task<Result> RemoveMemberAsync(Guid projectId, Guid userId, Guid requestingUserId)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(projectId);
            if (project is null)
                return Result.Failure("Project not found.");

            var requester = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (requester is null || requester.Role == ProjectRole.Contributor)
                return Result.Failure("You don't have permission to remove members.");

            var member = project.Members.FirstOrDefault(m => m.UserId == userId);
            if (member is not null)
            {
                var assignedTasks = await uow.Tasks.GetByMemberAsync(member.Id);
                foreach (var task in assignedTasks)
                {
                    task.Unassign();
                    uow.Tasks.Update(task);
                }
            }

            project.RemoveMember(userId);
            uow.Projects.Update(project);
            await uow.CommitAsync();

            return Result.Success();
        }

        private async Task<Result<ProjectDTO>> ChangeStatusAsync(
            Guid id, Guid requestingUserId, Action<Domain.Entities.Project> statusChange)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(id);
            if (project is null)
                return Result<ProjectDTO>.Failure("Project not found.");

            var member = project.Members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (member is null || member.Role != ProjectRole.Owner)
                return Result<ProjectDTO>.Failure("Only the project owner can change the project status.");

            try
            {
                statusChange(project);
            }
            catch (DomainException ex)
            {
                return Result<ProjectDTO>.Failure(ex.Message);
            }

            uow.Projects.Update(project);
            await uow.CommitAsync();

            return Result<ProjectDTO>.Success(mapper.Map<ProjectDTO>(project));
        }

        public async Task<Result<IEnumerable<ProjectMemberDTO>>> GetMembersAsync(Guid projectId, Guid requestingUserId)
        {
            var project = await uow.Projects.GetByIdWithMembersAsync(projectId);
            if (project is null)
                return Result<IEnumerable<ProjectMemberDTO>>.Failure("Project not found.");

            var members = project.Members;
            var requester = members.FirstOrDefault(m => m.UserId == requestingUserId);
            if (requester is null)
                return Result<IEnumerable<ProjectMemberDTO>>.Failure("You don't have permission to get members.");

            return Result<IEnumerable<ProjectMemberDTO>>.Success(mapper.Map<IEnumerable<ProjectMemberDTO>>(members));
        }
    }
}
