using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPro.Application.DTOs.Users;
using TaskPro.Application.Interfaces;

namespace TaskPro.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Get all users")]
        [EndpointDescription("Returns all registered users in the system. Restricted to Admin role.")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await userService.GetAllAsync();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("assignable")]
        [Authorize(Roles = "Admin,Manager")]
        [EndpointSummary("Get assignable users")]
        [EndpointDescription("Returns users with Manager or Member role that can be assigned to projects. Restricted to Admin and Manager roles.")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAssignable()
        {
            var result = await userService.GetAssignableAsync();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        [EndpointSummary("Get user by ID")]
        [EndpointDescription("Returns the details of a specific user by their ID.")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await userService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Create user")]
        [EndpointDescription("Creates a new user account. Restricted to Admin role.")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO dto)
        {
            var result = await userService.CreateAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
                : BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        [EndpointSummary("Update user")]
        [EndpointDescription("Updates the first and last name of a user. Admins can update any user, regular users can only update themselves.")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO dto)
        {
            var requestingUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && requestingUserId != id)
                return Forbid();

            var result = await userService.UpdateAsync(id, dto);
            return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [EndpointSummary("Delete user")]
        [EndpointDescription("Permanently deletes a user from the system. Restricted to Admin role.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await userService.DeleteAsync(id);
            return result.IsSuccess ? NoContent() : NotFound(new { error = result.Error });
        }
    }
}