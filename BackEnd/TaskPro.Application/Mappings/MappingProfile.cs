using AutoMapper;
using TaskPro.Application.DTOs.Projects;
using TaskPro.Application.DTOs.Tasks;
using TaskPro.Application.DTOs.Users;
using TaskPro.Domain.Entities;

namespace TaskPro.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.Name.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.Name.LastName))
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name.DisplayName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Value))
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

            CreateMap<ProjectMember, ProjectMemberDTO>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.Name.DisplayName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.User.Email.Value))
                .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
                .ForMember(d => d.JoinedAt, o => o.MapFrom(s => s.CreatedAt));

            CreateMap<Project, ProjectDTO>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.OwnerName, o => o.MapFrom(s => s.Owner != null
                    ? s.Owner.Name.DisplayName : string.Empty))
                .ForMember(d => d.TaskCount, o => o.MapFrom(s => s.Tasks.Count))
                .ForMember(d => d.CompletedTaskCount, o => o.MapFrom(s =>
                    s.Tasks.Count(t => t.Status == Domain.Enums.TaskItemStatus.Done)));

            CreateMap<TaskItem, TaskItemDTO>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.ProjectName, o => o.MapFrom(s => s.Project != null
                    ? s.Project.Name : string.Empty))
                .ForMember(d => d.AssignedToName, o => o.MapFrom(s =>
                    s.AssignedTo != null
                        ? s.AssignedTo.User.Name.DisplayName
                        : "Unassigned"));
        }
    }
}
