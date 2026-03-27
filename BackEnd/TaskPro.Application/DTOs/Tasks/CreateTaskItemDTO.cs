namespace TaskPro.Application.DTOs.Tasks
{
    public class CreateTaskItemDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
