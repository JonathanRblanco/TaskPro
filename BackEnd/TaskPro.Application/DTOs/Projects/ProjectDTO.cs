namespace TaskPro.Application.DTOs.Projects
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public List<ProjectMemberDTO> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
