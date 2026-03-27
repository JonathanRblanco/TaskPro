using TaskPro.Domain.Entities.Base;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.Entities
{
    public class Project : BaseEntity
    {
        private Project()
        {
            
        }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public ProjectStatus Status { get; private set; }
        public Guid OwnerId { get; private set; }
        public User Owner { get; private set; } = null!;

        private readonly List<ProjectMember> _members = [];
        public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

        private readonly List<TaskItem> _tasks = [];
        public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

        public static Project Create(string name, Guid ownerId, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Project name is required.");

            return new Project
            {
                Name = name.Trim(),
                Description = description?.Trim() ?? string.Empty,
                OwnerId = ownerId,
                Status = ProjectStatus.Active
            };
        }

        public ProjectMember AddMember(Guid userId, ProjectRole role = ProjectRole.Contributor)
        {
            if (_members.Any(m => m.UserId == userId))
                throw new ConflictException("User is already a member of this project.");

            var member = ProjectMember.Create(Id, userId, role);
            _members.Add(member);
            return member;
        }

        public void Complete()
        {
            if (Status != ProjectStatus.Active)
                throw new DomainException("Only active projects can be completed.");

            var hasUnfinishedTasks = _tasks.Any(t =>
                t.Status != TaskItemStatus.Done &&
                t.Status != TaskItemStatus.Cancelled);

            if (hasUnfinishedTasks)
                throw new DomainException("Cannot complete a project with unfinished tasks.");

            Status = ProjectStatus.Completed;
            MarkAsUpdated();
        }

        public void Cancel()
        {
            if (Status == ProjectStatus.Archived)
                throw new DomainException("Cannot cancel an archived project.");

            if (Status == ProjectStatus.Cancelled)
                throw new DomainException("Project is already cancelled.");

            Status = ProjectStatus.Cancelled;
            MarkAsUpdated();
        }

        public void Archive()
        {
            if (Status == ProjectStatus.Active)
                throw new DomainException("Cannot archive an active project. Complete or cancel it first.");

            if (Status == ProjectStatus.Archived)
                throw new DomainException("Project is already archived.");

            Status = ProjectStatus.Archived;
            MarkAsUpdated();
        }

        public void Reopen()
        {
            if (Status != ProjectStatus.Completed && Status != ProjectStatus.Cancelled)
                throw new DomainException("Only completed or cancelled projects can be reopened.");

            Status = ProjectStatus.Active;
            MarkAsUpdated();
        }

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void RemoveMember(Guid userId)
        {
            var member = _members.FirstOrDefault(u => u.UserId == userId);
            if (member is null)
                throw new NotFoundException($"There is not member with Id: {userId}");
            _members.Remove(member);
        }
    }
}
