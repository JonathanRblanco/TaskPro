using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Comments;
using TaskPro.Application.Interfaces;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Application.Services
{
    public class CommentService(
        IUnitOfWork uow,
        ICommentsRepository commentsRepo) : ICommentService
    {
        public async Task<Result<IEnumerable<CommentDTO>>> GetByTaskAsync(
            Guid taskId, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdAsync(taskId);
            if (task is null)
                return Result<IEnumerable<CommentDTO>>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null)
                return Result<IEnumerable<CommentDTO>>.Failure("You are not a member of this project.");

            var comments = await commentsRepo.GetByTaskAsync(taskId);

            var userIds = comments.Select(c => c.UserId).Distinct().ToList();
            var users = new Dictionary<Guid, string>();

            foreach (var uid in userIds)
            {
                var user = await uow.Users.GetByIdAsync(uid);
                if (user is not null)
                    users[uid] = user.Name.DisplayName;
            }

            var DTOs = comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                TaskId = c.TaskId,
                UserId = c.UserId,
                UserName = users.GetValueOrDefault(c.UserId, "Unknown"),
                Content = c.Content,
                CreatedAt = c.CreatedAt,
            });

            return Result<IEnumerable<CommentDTO>>.Success(DTOs);
        }

        public async Task<Result<CommentDTO>> AddAsync(
            Guid taskId, CreateCommentDTO DTO, Guid requestingUserId)
        {
            var task = await uow.Tasks.GetByIdAsync(taskId);
            if (task is null)
                return Result<CommentDTO>.Failure("Task not found.");

            var member = await uow.Projects.GetMemberAsync(task.ProjectId, requestingUserId);
            if (member is null)
                return Result<CommentDTO>.Failure("You are not a member of this project.");

            var comment = Comment.Create(taskId, requestingUserId, DTO.Content);
            await commentsRepo.AddAsync(comment);

            var user = await uow.Users.GetByIdAsync(requestingUserId);

            return Result<CommentDTO>.Success(new CommentDTO
            {
                Id = comment.Id,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserName = user?.Name.DisplayName ?? "Unknown",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
            });
        }

        public async Task<Result> DeleteAsync(Guid commentId, Guid requestingUserId)
        {
            var comment = await commentsRepo.GetByIdAsync(commentId);
            if (comment is null)
                return Result.Failure("Comment not found.");

            if (comment.UserId != requestingUserId)
                return Result.Failure("You can only delete your own comments.");

            await commentsRepo.DeleteAsync(commentId);
            return Result.Success();
        }
    }
}
