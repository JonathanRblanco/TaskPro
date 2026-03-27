using TaskPro.Domain.Entities.Base;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        private TaskItem()
        {
            
        }
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = string.Empty;
        public TaskItemStatus Status { get; private set; }
        public DateTime? DueDate { get; private set; }

        public Guid ProjectId { get; private set; }
        public Project Project { get; private set; } = null!;
        public Guid? AssignedToMemberId { get; private set; }
        public ProjectMember? AssignedTo { get; private set; }

        public static TaskItem Create(string title, Guid projectId,
                                      string? description = null,
                                      DateTime? dueDate = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Task title is required.");

            if (dueDate.HasValue && dueDate.Value.ToUniversalTime() <= DateTime.UtcNow)
                throw new DomainException("Due date must be in the future.");

            return new TaskItem
            {
                Title = title.Trim(),
                Description = description?.Trim() ?? string.Empty,
                ProjectId = projectId,
                DueDate = dueDate,
                Status = TaskItemStatus.Pending
            };
        }

        public void AssignTo(Guid memberId)
        {
            AssignedToMemberId = memberId;
            MarkAsUpdated();
        }

        public void Unassign()
        {
            AssignedToMemberId = null;
            MarkAsUpdated();
        }
        public void ChangeStatus(TaskItemStatus newStatus)
        {
            if (Status == TaskItemStatus.Cancelled)
                throw new DomainException("Cannot change the status of a cancelled task.");

            if (Status == newStatus)
                throw new DomainException($"Task is already in '{newStatus}' status.");

            Status = newStatus;
            MarkAsUpdated();
        }

        public void UpdateDetails(string title, string? description, DateTime? dueDate)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Task title is required.");

            if (dueDate.HasValue && dueDate.Value.ToUniversalTime() <= DateTime.UtcNow)
                throw new DomainException("Due date must be in the future.");

            Title = title.Trim();
            Description = description?.Trim() ?? string.Empty;
            DueDate = dueDate;
            MarkAsUpdated();
        }
    }
}
