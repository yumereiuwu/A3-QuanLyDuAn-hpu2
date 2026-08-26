using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Services;
using System.Security.Claims;

namespace HRManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly IUserRepository _userRepository;

        public AuthController(IAuthService authService, IJwtService jwtService, IUserRepository userRepository)
        {
            _authService = authService;
            _jwtService = jwtService;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.AuthenticateAsync(request.Username, request.Password);
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = _jwtService.GenerateToken(user);
                
                // Log token generation for debugging
                Console.WriteLine($"Token generated for user: {user.Username}, Role: {user.Role}");
                Console.WriteLine($"Token: {token}");

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        fullName = user.FullName,
                        role = user.Role,
                        department = user.Department,
                        position = user.Position
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var success = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
                if (!success)
                {
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    fullName = user.FullName,
                    phone = user.Phone,
                    address = user.Address,
                    gender = user.Gender,
                    dateOfBirth = user.DateOfBirth,
                    department = user.Department,
                    position = user.Position,
                    hireDate = user.HireDate,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var updatedUser = new User
                {
                    Phone = request.Phone,
                    Address = request.Address
                };

                var success = await _authService.UpdateProfileAsync(userId, updatedUser);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update profile" });
                }

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In a real application, you might want to blacklist the token
            // For now, we'll just return a success message
            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _userRepository.GetAllAsync();
                var employeeList = employees.Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    email = u.Email,
                    phone = u.Phone,
                    address = u.Address,
                    gender = u.Gender,
                    dateOfBirth = u.DateOfBirth,
                    department = u.Department,
                    position = u.Position,
                    hireDate = u.HireDate,
                    role = u.Role,
                    status = u.IsActive ? "active" : "inactive",
                    createdAt = u.CreatedAt
                }).ToList();

                return Ok(employeeList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("employees")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeRequest request)
        {
            try
            {
                // Basic validations
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Username) ||
                    string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Department) || string.IsNullOrWhiteSpace(request.Position))
                {
                    return BadRequest(new { message = "Thiếu dữ liệu bắt buộc" });
                }

                // Normalize
                request.Email = request.Email.Trim();
                request.Username = request.Username.Trim();

                // Check if email already exists; if trùng với user đã xóa mềm, giải phóng tự động
                if (!await _userRepository.IsEmailUniqueAsync(request.Email))
                {
                    var existingByEmail = await _userRepository.GetByEmailAsync(request.Email);
                    if (existingByEmail != null && existingByEmail.IsActive == false)
                    {
                        var suffix = $"__deleted_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
                        existingByEmail.Email = existingByEmail.Email + suffix;
                        existingByEmail.Username = existingByEmail.Username + suffix;
                        await _userRepository.UpdateAsync(existingByEmail.Id, existingByEmail);
                    }
                    else
                    {
                        return BadRequest(new { message = "Email đã tồn tại" });
                    }
                }

                // Check if username already exists; if trùng với user đã xóa mềm, giải phóng tự động
                if (!await _userRepository.IsUsernameUniqueAsync(request.Username))
                {
                    var existingByUsername = await _userRepository.GetByUsernameAsync(request.Username);
                    if (existingByUsername != null && existingByUsername.IsActive == false)
                    {
                        var suffix2 = $"__deleted_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
                        existingByUsername.Username = existingByUsername.Username + suffix2;
                        // Email có thể đã khác, không chỉnh nếu không trùng
                        await _userRepository.UpdateAsync(existingByUsername.Id, existingByUsername);
                    }
                    else
                    {
                        return BadRequest(new { message = "Username đã tồn tại" });
                    }
                }

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Phone = request.Phone,
                    Address = request.Address,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    Department = request.Department,
                    Position = request.Position,
                    HireDate = request.HireDate,
                    Role = request.Role ?? "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                string userId;
                try
                {
                    userId = await _userRepository.AddAsync(user);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
                {
                    // Unique constraint or other DB error
                    return BadRequest(new { message = "Không thể thêm nhân viên (trùng email/username hoặc dữ liệu không hợp lệ)", error = dbEx.InnerException?.Message ?? dbEx.Message });
                }

                return Ok(new { message = "Employee added successfully", id = userId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("employees/{id}")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                // Check if email already exists (excluding current user)
                if (request.Email != existingUser.Email && !await _userRepository.IsEmailUniqueAsync(request.Email))
                {
                    return BadRequest(new { message = "Email already exists" });
                }

                existingUser.FullName = request.FullName;
                existingUser.Email = request.Email;
                existingUser.Phone = request.Phone;
                existingUser.Address = request.Address;
                existingUser.Gender = request.Gender;
                existingUser.DateOfBirth = request.DateOfBirth;
                existingUser.Department = request.Department;
                existingUser.Position = request.Position;
                existingUser.HireDate = request.HireDate;
                existingUser.Role = request.Role ?? existingUser.Role;

                var success = await _userRepository.UpdateAsync(id, existingUser);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to update employee" });
                }

                return Ok(new { message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            try
            {
                var success = await _userRepository.DeleteAsync(id);
                if (!success)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UpdateProfileRequest
    {
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class AddEmployeeRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string? Role { get; set; }
    }

    public class UpdateEmployeeRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public string? Role { get; set; }
    }
}
