namespace TaskPro.Application.DTOs.Tasks
{
    public class UpdateTaskItemDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
