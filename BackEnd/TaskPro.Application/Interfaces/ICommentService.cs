using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Comments;

namespace TaskPro.Application.Interfaces
{
    public interface ICommentService
    {
        Task<Result<IEnumerable<CommentDTO>>> GetByTaskAsync(Guid taskId, Guid requestingUserId);
        Task<Result<CommentDTO>> AddAsync(Guid taskId, CreateCommentDTO dto, Guid requestingUserId);
        Task<Result> DeleteAsync(Guid commentId, Guid requestingUserId);
    }
}
