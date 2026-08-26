using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<User?> GetUserByIdAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, User updatedUser);
        Task<bool> ValidatePasswordAsync(string userId, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null || !user.IsActive)
                {
                    return null;
                }

                if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for username: {Username}", username);
                throw;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                return await _userRepository.UpdateAsync(userId, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _userRepository.GetByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> UpdateProfileAsync(string userId, User updatedUser)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(userId);
                if (existingUser == null)
                {
                    return false;
                }

                // Update only allowed fields
                existingUser.Phone = updatedUser.Phone;
                existingUser.Address = updatedUser.Address;
                existingUser.UpdatedAt = DateTime.UtcNow;

                return await _userRepository.UpdateAsync(userId, existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ValidatePasswordAsync(string userId, string password)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating password for user: {UserId}", userId);
                throw;
            }
        }
    }
}
