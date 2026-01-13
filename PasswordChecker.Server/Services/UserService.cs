using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.User;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                Varsta = dto.Varsta,
                Gen = dto.Gen,
                Status = "ACTIVE",
                Role = "CUSTOMER",
                Balance = 100,
                CreatedAt = DateTime.Now
            };

            var created = await _userRepository.AddAsync(user);
            return MapToDto(created);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Varsta = user.Varsta,
                Gen = user.Gen,
                Status = user.Status,
                Role = user.Role,
                Balance = user.Balance
            };
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            return MapToDto(user);
        }
        public async Task<UserDto> AddBalanceAsync(Guid userId, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            user.Balance += amount;

            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

    }
}
