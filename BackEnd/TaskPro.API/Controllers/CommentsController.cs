using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPro.Application.DTOs.Comments;
using TaskPro.Application.Interfaces;

namespace TaskPro.API.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId:guid}/comments")]
    [Authorize]
    public class CommentsController(ICommentService commentService) : ControllerBase
    {
        private Guid RequestingUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [EndpointSummary("Get comments by task")]
        [EndpointDescription("Returns all comments associated with the specified task.")]
        [ProducesResponseType(typeof(IEnumerable<CommentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByTask(Guid taskId)
        {
            var result = await commentService.GetByTaskAsync(taskId, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPost]
        [EndpointSummary("Add comment")]
        [EndpointDescription("Adds a new comment to the specified task.")]
        [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(Guid taskId, [FromBody] CreateCommentDTO dto)
        {
            var result = await commentService.AddAsync(taskId, dto, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpDelete("{commentId}")]
        [EndpointSummary("Delete comment")]
        [EndpointDescription("Deletes the specified comment. Only the comment author can delete it.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid taskId, Guid commentId)
        {
            var result = await commentService.DeleteAsync(commentId, RequestingUserId);
            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
        }
    }
}