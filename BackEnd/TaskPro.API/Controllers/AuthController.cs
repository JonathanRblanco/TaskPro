using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskPro.Application.DTOs.Auth;
using TaskPro.Application.Interfaces;

namespace TaskPro.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private Guid RequestingUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("login")]
        [AllowAnonymous]
        [EndpointSummary("Login")]
        [EndpointDescription("Authenticates a user with email and password and returns a JWT token.")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await authService.LoginAsync(dto);
            return result.IsSuccess
                ? Ok(result.Value)
                : Unauthorized(new { error = result.Error });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EndpointSummary("Register")]
        [EndpointDescription("Creates a new user account and returns a JWT token.")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            var result = await authService.RegisterAsync(dto);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Login), result.Value)
                : BadRequest(new { error = result.Error });
        }

        [HttpPost("refresh")]
        [Authorize]
        [EndpointSummary("Refresh token")]
        [EndpointDescription("Generates a new JWT token for the currently authenticated user.")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            var result = await authService.RefreshTokenAsync(RequestingUserId);
            return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
        }
    }
}