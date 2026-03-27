using TaskPro.Domain.Exceptions;

namespace TaskPro.Domain.Entities
{
    public class Comment
    {
        private Comment()
        {
            
        }
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static Comment Create(Guid taskId, Guid userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Comment content cannot be empty.");

            if (content.Trim().Length > 2000)
                throw new DomainException("Comment cannot exceed 2000 characters.");

            return new Comment
            {
                TaskId = taskId,
                UserId = userId,
                Content = content.Trim()
            };
        }

        public void Edit(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
                throw new DomainException("Comment content cannot be empty.");

            if (newContent.Trim().Length > 2000)
                throw new DomainException("Comment cannot exceed 2000 characters.");

            Content = newContent.Trim();
        }
    }
}
