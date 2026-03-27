namespace TaskPro.Application.DTOs.Projects
{
    public class AddMemberDTO
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = "Contributor";
    }
}
