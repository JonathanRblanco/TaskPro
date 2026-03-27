using TaskPro.Domain.Entities.Base;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.Entities
{
    public class ProjectMember : BaseEntity
    {
        private ProjectMember()
        {
            
        }
        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public ProjectRole Role { get; private set; }

        private readonly List<TaskItem> _assignedTasks = [];
        public IReadOnlyCollection<TaskItem> AssignedTasks => _assignedTasks.AsReadOnly();

        internal static ProjectMember Create(Guid projectId, Guid userId, ProjectRole role)
        {
            return new ProjectMember
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                UserId = userId,
                Role = role
            };
        }

        public void ChangeRole(ProjectRole newRole)
        {
            if (Role == ProjectRole.Owner)
                throw new DomainException("Cannot change the role of the project owner.");

            Role = newRole;
            MarkAsUpdated();
        }
    }
}
