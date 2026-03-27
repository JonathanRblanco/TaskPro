using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Users;

namespace TaskPro.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDTO>>> GetAllAsync();
        Task<Result<UserDTO>> GetByIdAsync(Guid id);
        Task<Result<UserDTO>> CreateAsync(CreateUserDTO dto);
        Task<Result<UserDTO>> UpdateAsync(Guid id, UpdateUserDTO dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<IEnumerable<UserDTO>>> GetAssignableAsync();
    }
}
