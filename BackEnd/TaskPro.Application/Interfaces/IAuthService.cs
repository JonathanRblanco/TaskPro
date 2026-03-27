using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Auth;

namespace TaskPro.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO dto);
        Task<Result<AuthResponseDTO>> RefreshTokenAsync(Guid userId);
        Task<Result<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO dto);
    }
}
