using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPro.Application.DTOs.Tasks;
using TaskPro.Application.Interfaces;

namespace TaskPro.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    [Authorize]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        private Guid RequestingUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [EndpointSummary("Get tasks by project")]
        [EndpointDescription("Returns all tasks belonging to the specified project.")]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByProject([FromQuery] Guid projectId)
        {
            var result = await taskService.GetByProjectAsync(projectId, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("my-tasks")]
        [EndpointSummary("Get my tasks")]
        [EndpointDescription("Returns all tasks assigned to the currently authenticated user.")]
        [ProducesResponseType(typeof(IEnumerable<TaskItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUser()
        {
            var result = await taskService.GetByUserAsync(RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Get task by ID")]
        [EndpointDescription("Returns the details of a specific task by its ID.")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await taskService.GetByIdAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
        }

        [HttpPost]
        [EndpointSummary("Create task")]
        [EndpointDescription("Creates a new task and assigns it to a project.")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTaskItemDTO dto)
        {
            var result = await taskService.CreateAsync(dto, RequestingUserId);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
                : BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        [EndpointSummary("Update task")]
        [EndpointDescription("Updates the title and description of an existing task.")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskItemDTO dto)
        {
            var result = await taskService.UpdateAsync(id, dto, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        [EndpointSummary("Delete task")]
        [EndpointDescription("Permanently deletes the specified task.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await taskService.DeleteAsync(id, RequestingUserId);
            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/status")]
        [EndpointSummary("Change task status")]
        [EndpointDescription("Updates the status of a task (Pending, InProgress, Done, Cancelled).")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeTaskStatusDTO dto)
        {
            var result = await taskService.ChangeStatusAsync(id, dto.Status, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/assign")]
        [EndpointSummary("Assign task")]
        [EndpointDescription("Assigns a task to a specific user.")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTaskDTO dto)
        {
            var result = await taskService.AssignAsync(id, dto.UserId, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/unassign")]
        [EndpointSummary("Unassign task")]
        [EndpointDescription("Removes the current assignee from the specified task.")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Unassign(Guid id)
        {
            var result = await taskService.UnassignAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }
    }
}