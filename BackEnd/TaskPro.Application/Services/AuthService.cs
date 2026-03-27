using TaskPro.Application.Common;
using TaskPro.Application.DTOs.Auth;
using TaskPro.Application.Interfaces;
using TaskPro.Domain.Entities;
using TaskPro.Domain.Interfaces.Repositories;

namespace TaskPro.Application.Services
{
    public class AuthService(IUnitOfWork uow, IJwtService jwtService) : IAuthService
    {
        public async Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO dto)
        {
            var user = await uow.Users.GetByEmailAsync(dto.Email);
            if (user is null)
                return Result<AuthResponseDTO>.Failure("Invalid email or password.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordValid)
                return Result<AuthResponseDTO>.Failure("Invalid email or password.");

            var token = jwtService.GenerateToken(
                userId: user.Id,
                email: user.Email.Value,
                role: user.Role.ToString());

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                Token = token,
                Email = user.Email.Value,
                FullName = user.Name.DisplayName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }

        public async Task<Result<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO dto)
        {
            var exists = await uow.Users.ExistsWithEmailAsync(dto.Email);
            if (exists)
                return Result<AuthResponseDTO>.Failure("Email is already registered.");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = User.Create(dto.FirstName, dto.LastName, dto.Email, hash);

            await uow.Users.AddAsync(user);
            await uow.CommitAsync();

            var token = jwtService.GenerateToken(
                userId: user.Id,
                email: user.Email.Value,
                role: user.Role.ToString());

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                Token = token,
                Email = user.Email.Value,
                FullName = user.Name.DisplayName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }

        public async Task<Result<AuthResponseDTO>> RefreshTokenAsync(Guid userId)
        {
            var user = await uow.Users.GetByIdAsync(userId);
            if (user is null)
                return Result<AuthResponseDTO>.Failure("User not found.");

            var token = jwtService.GenerateToken(
                userId: user.Id,
                email: user.Email.Value,
                role: user.Role.ToString());

            return Result<AuthResponseDTO>.Success(new AuthResponseDTO
            {
                Token = token,
                Email = user.Email.Value,
                FullName = user.Name.DisplayName,
                Role = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}
