using AutoMapper;
using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Users;
using TaskPro.Application.Interfaces;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Enums;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Application.Services
{
    public class UserService(IUnitOfWork uow, IMapper mapper) : IUserService
    {
        public async Task<Result<IEnumerable<UserDTO>>> GetAllAsync()
        {
            var users = await uow.Users.GetAllAsync();
            return Result<IEnumerable<UserDTO>>.Success(mapper.Map<IEnumerable<UserDTO>>(users));
        }

        public async Task<Result<UserDTO>> GetByIdAsync(Guid id)
        {
            var user = await uow.Users.GetByIdAsync(id);
            if (user is null)
                return Result<UserDTO>.Failure("User not found.");

            return Result<UserDTO>.Success(mapper.Map<UserDTO>(user));
        }

        public async Task<Result<UserDTO>> CreateAsync(CreateUserDTO DTO)
        {
            var exists = await uow.Users.ExistsWithEmailAsync(DTO.Email);
            if (exists)
                return Result<UserDTO>.Failure("Email is already registered.");

            if (!Enum.TryParse<ApplicationRole>(DTO.Role, out var role))
                return Result<UserDTO>.Failure("Invalid role.");

            var hash = BCrypt.Net.BCrypt.HashPassword(DTO.Password);
            var user = User.Create(DTO.FirstName, DTO.LastName, DTO.Email, hash, role);

            await uow.Users.AddAsync(user);
            await uow.CommitAsync();

            return Result<UserDTO>.Success(mapper.Map<UserDTO>(user));
        }

        public async Task<Result<UserDTO>> UpdateAsync(Guid id, UpdateUserDTO DTO)
        {
            var user = await uow.Users.GetByIdAsync(id);
            if (user is null)
                return Result<UserDTO>.Failure("User not found.");

            user.UpdateProfile(DTO.FirstName, DTO.LastName);
            uow.Users.Update(user);
            await uow.CommitAsync();

            return Result<UserDTO>.Success(mapper.Map<UserDTO>(user));
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var user = await uow.Users.GetByIdAsync(id);
            if (user is null)
                return Result.Failure("User not found.");

            uow.Users.Delete(user);
            await uow.CommitAsync();

            return Result.Success();
        }

        public async Task<Result<IEnumerable<UserDTO>>> GetAssignableAsync()
        {
            var users = await uow.Users.GetByRolesAsync([ApplicationRole.Manager, ApplicationRole.Member]);
            return Result<IEnumerable<UserDTO>>.Success(mapper.Map<IEnumerable<UserDTO>>(users));
        }
    }
}
