using Microsoft.Extensions.Logging;
using PasswordChecker.Data.Models;
using PasswordChecker.Data.Repositories.Interfaces;
using PasswordChecker.Server.DTOs.User;
using PasswordChecker.Server.Services.Interfaces;

namespace PasswordChecker.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCacheService _cacheService;
        private readonly ILogger<UserService> _logger;

        private const string STATUS_ACTIVE = "ACTIVE";
        private const string ROLE_CUSTOMER = "CUSTOMER";
        private const string CACHE_KEY_ALL_USERS = "users_all";
        private const string CACHE_KEY_USER_BY_ID = "user_{0}";
        private const string CACHE_KEY_USER_BY_EMAIL = "user_email_{0}";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromMinutes(5);

        public UserService(
            IUserRepository userRepository,
            IMemoryCacheService cacheService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        // READ
        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            _logger.LogInformation("Getting all users");

            return await _cacheService.GetOrSetAsync(
                CACHE_KEY_ALL_USERS,
                async () =>
                {
                    _logger.LogInformation("Fetching all users from repository");
                    var users = await _userRepository.GetAllAsync();
                    var userDtos = users.Select(MapToDto).ToList();
                    _logger.LogInformation("Fetched {Count} users from repository", userDtos.Count);
                    return userDtos;
                },
                CACHE_DURATION) ?? Enumerable.Empty<UserDto>();
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user by ID: {UserId}", id);

            var cacheKey = string.Format(CACHE_KEY_USER_BY_ID, id);
            
            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    _logger.LogInformation("Fetching user {UserId} from repository", id);
                    var user = await _userRepository.GetByIdAsync(id);
                    if (user == null)
                    {
                        _logger.LogWarning("User {UserId} not found", id);
                        return null;
                    }
                    return MapToDto(user);
                },
                CACHE_DURATION);
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            _logger.LogInformation("Getting user by email: {Email}", email);

            var normalizedEmail = email.Trim().ToLower();
            var cacheKey = string.Format(CACHE_KEY_USER_BY_EMAIL, normalizedEmail);

            return await _cacheService.GetOrSetAsync(
                cacheKey,
                async () =>
                {
                    _logger.LogInformation("Fetching user with email {Email} from repository", email);
                    var user = await _userRepository.GetByEmailAsync(normalizedEmail);
                    if (user == null)
                    {
                        _logger.LogWarning("User with email {Email} not found", email);
                        return null;
                    }
                    return MapToDto(user);
                },
                CACHE_DURATION);
        }

        // CREATE
        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            _logger.LogInformation("Creating new user with email: {Email}", dto.Email);

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                _logger.LogWarning("Attempted to create user with empty email");
                throw new InvalidOperationException("Email is required");
            }

            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
            {
                _logger.LogWarning("Attempted to create user with existing email: {Email}", dto.Email);
                throw new InvalidOperationException("Email already exists");
            }

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
            _logger.LogInformation("User created successfully with ID: {UserId}", created.Id);

            // Invalidate cache
            InvalidateUserCache(created.Id, created.Email);

            return MapToDto(created);
        }

        // UPDATE
        public async Task<UserDto> UpdateAsync(UpdateUserDto dto)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", dto.Id);

            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for update", dto.Id);
                throw new InvalidOperationException("User not found");
            }

            user.Varsta = dto.Varsta;
            user.Gen = dto.Gen;
            user.Status = dto.Status;
            user.Role = dto.Role;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User {UserId} updated successfully", dto.Id);

            // Invalidate cache
            InvalidateUserCache(user.Id, user.Email);

            return MapToDto(user);
        }

        public async Task<UserDto> AddBalanceAsync(Guid userId, decimal amount)
        {
            _logger.LogInformation("Adding balance {Amount} to user {UserId}", amount, userId);

            if (amount <= 0)
            {
                _logger.LogWarning("Attempted to add invalid amount {Amount} to user {UserId}", amount, userId);
                throw new InvalidOperationException("Amount must be greater than zero");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found for balance update", userId);
                throw new InvalidOperationException("User not found");
            }

            var oldBalance = user.Balance;
            user.Balance += amount;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Balance updated for user {UserId}: {OldBalance} -> {NewBalance}", 
                userId, oldBalance, user.Balance);

            // Invalidate cache
            InvalidateUserCache(user.Id, user.Email);

            return MapToDto(user);
        }

        // DELETE
        public async Task DeleteAsync(Guid id)
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var user = await _userRepository.GetByIdAsync(id);
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("User {UserId} not found for deletion", id);
                throw new InvalidOperationException("User not found");
            }

            _logger.LogInformation("User {UserId} deleted successfully", id);

            // Invalidate cache
            if (user != null)
            {
                InvalidateUserCache(user.Id, user.Email);
            }
        }

        private void InvalidateUserCache(Guid userId, string email)
        {
            _cacheService.Remove(CACHE_KEY_ALL_USERS);
            _cacheService.Remove(string.Format(CACHE_KEY_USER_BY_ID, userId));
            _cacheService.Remove(string.Format(CACHE_KEY_USER_BY_EMAIL, email.Trim().ToLower()));
            _logger.LogInformation("Cache invalidated for user {UserId}", userId);
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
