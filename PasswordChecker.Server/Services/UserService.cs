using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.User;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private const string STATUS_ACTIVE = "ACTIVE";
        private const string ROLE_CUSTOMER = "CUSTOMER";

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // READ
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto);
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        // CREATE
        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new InvalidOperationException("Email is required");

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email.Trim().ToLower(),
                Varsta = dto.Varsta,
                Gen = dto.Gen,
                Status = STATUS_ACTIVE,
                Role = ROLE_CUSTOMER,
                Balance = 0,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.AddAsync(user);
            return MapToDto(created);
        }

        // UPDATE
        public async Task<UserDto> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Varsta = dto.Varsta;
            user.Gen = dto.Gen;
            user.Status = dto.Status;
            user.Role = dto.Role;

            await _userRepository.UpdateAsync(user);
            return MapToDto(user);
        }

        public async Task<UserDto> AddBalanceAsync(Guid userId, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount must be greater than zero");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Balance += amount;
            await _userRepository.UpdateAsync(user);

            return MapToDto(user);
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                throw new InvalidOperationException("User not found");
        }

        // MAPPING
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
    }
}
