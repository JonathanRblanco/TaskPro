using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPro.Application.DTOs.Projects;
using TaskPro.Application.Interfaces;

namespace TaskPro.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private Guid RequestingUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all projects")]
        [EndpointDescription("Returns all projects in the system. Restricted to Admin role.")]
        [ProducesResponseType(typeof(IEnumerable<ProjectDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await projectService.GetAllAsync();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("my-projects")]
        [EndpointSummary("Get my projects")]
        [EndpointDescription("Returns all projects assigned to the currently authenticated user.")]
        [ProducesResponseType(typeof(IEnumerable<ProjectDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUser()
        {
            var result = await projectService.GetByUserAsync(RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Get project by ID")]
        [EndpointDescription("Returns the details of a specific project by its ID.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await projectService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Create project")]
        [EndpointDescription("Creates a new project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProjectDTO dto)
        {
            var result = await projectService.CreateAsync(dto, RequestingUserId);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
                : BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Update project")]
        [EndpointDescription("Updates the details of an existing project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDTO dto)
        {
            var result = await projectService.UpdateAsync(id, dto, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Delete project")]
        [EndpointDescription("Permanently deletes a project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await projectService.DeleteAsync(id, RequestingUserId);
            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/complete")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Complete project")]
        [EndpointDescription("Marks a project as completed. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Complete(Guid id)
        {
            var result = await projectService.CompleteAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/cancel")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Cancel project")]
        [EndpointDescription("Cancels an active project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await projectService.CancelAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPatch("{id:guid}/archive")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Archive project")]
        [EndpointDescription("Archives a completed or cancelled project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Archive(Guid id)
        {
            var result = await projectService.ArchiveAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}/members")]
        [EndpointSummary("Get project members")]
        [EndpointDescription("Returns all members belonging to the specified project.")]
        [ProducesResponseType(typeof(IEnumerable<ProjectMemberDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await projectService.GetMembersAsync(id, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPost("{id:guid}/members")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Add project member")]
        [EndpointDescription("Adds a user as a member of the specified project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(ProjectMemberDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMember(Guid id, [FromBody] AddMemberDTO dto)
        {
            var result = await projectService.AddMemberAsync(id, dto.UserId, dto.Role, RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Remove project member")]
        [EndpointDescription("Removes a user from the specified project. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var result = await projectService.RemoveMemberAsync(id, userId, RequestingUserId);
            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
        }
    }
}