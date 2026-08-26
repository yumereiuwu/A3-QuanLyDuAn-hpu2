using Microsoft.AspNetCore.Mvc;
using HRManagement.Core.Interfaces;
using HRManagement.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace HRManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly ILogger<SalaryController> _logger;

        public SalaryController(ISalaryRepository salaryRepository, ILogger<SalaryController> logger)
        {
            _salaryRepository = salaryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalaries()
        {
            try
            {
                var salaries = await _salaryRepository.GetAllAsync();
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all salaries");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách lương" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalaryById(string id)
        {
            try
            {
                var salary = await _salaryRepository.GetByIdAsync(id);
                if (salary == null)
                {
                    return NotFound(new { message = "Không tìm thấy bản ghi lương" });
                }
                return Ok(salary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salary by id: {Id}", id);
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin lương" });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetSalariesByUserId(string userId)
        {
            try
            {
                _logger.LogInformation("GetSalariesByUserId called for user: {UserId}", userId);
                
                // Log current user info
                var currentUser = User.Identity?.Name;
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                _logger.LogInformation("Current user: {CurrentUser}, Role: {UserRole}", currentUser, userRole);
                
                var salaries = await _salaryRepository.GetByUserIdAsync(userId);
                _logger.LogInformation("Found {Count} salaries for user {UserId}", salaries?.Count() ?? 0, userId);
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salaries for user: {UserId}", userId);
                return StatusCode(500, new { message = "Lỗi khi lấy lương của nhân viên" });
            }
        }

        public class CreateSalaryRequest
        {
            public string UserId { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal BasicSalary { get; set; }
            public decimal Allowance { get; set; }
            public decimal Bonus { get; set; }
            public decimal OvertimePay { get; set; }
            public decimal Deductions { get; set; }
            public string? Notes { get; set; }
            public bool IsPaid { get; set; } = false;
            public DateTime? PaidDate { get; set; }
        }

        public class UpdateSalaryRequest : CreateSalaryRequest {}

        [HttpPost]
        public async Task<IActionResult> CreateSalary([FromBody] CreateSalaryRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new Salary
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    Department = request.Department,
                    Year = request.Year,
                    Month = request.Month,
                    BasicSalary = request.BasicSalary,
                    Allowance = request.Allowance,
                    Bonus = request.Bonus,
                    OvertimePay = request.OvertimePay,
                    Deductions = request.Deductions,
                    NetSalary = request.BasicSalary + request.Allowance + request.Bonus + request.OvertimePay - request.Deductions,
                    Notes = request.Notes,
                    IsPaid = request.IsPaid,
                    PaidDate = request.PaidDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var salaryId = await _salaryRepository.AddAsync(entity);
                return CreatedAtAction(nameof(GetSalaryById), new { id = salaryId }, entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating salary");
                return StatusCode(500, new { message = "Lỗi khi tạo bản ghi lương" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSalary(string id, [FromBody] UpdateSalaryRequest salary)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingSalary = await _salaryRepository.GetByIdAsync(id);
                if (existingSalary == null)
                {
                    return NotFound(new { message = "Không tìm thấy bản ghi lương" });
                }

                // Cập nhật thông tin
                existingSalary.UserId = salary.UserId;
                existingSalary.Department = salary.Department;
                existingSalary.Year = salary.Year;
                existingSalary.Month = salary.Month;
                existingSalary.BasicSalary = salary.BasicSalary;
                existingSalary.Allowance = salary.Allowance;
                existingSalary.Bonus = salary.Bonus;
                existingSalary.OvertimePay = salary.OvertimePay;
                existingSalary.Deductions = salary.Deductions;
                existingSalary.NetSalary = salary.BasicSalary + salary.Allowance + salary.Bonus + salary.OvertimePay - salary.Deductions;
                existingSalary.Notes = salary.Notes;
                existingSalary.IsPaid = salary.IsPaid;
                existingSalary.PaidDate = salary.PaidDate;
                existingSalary.UpdatedAt = DateTime.UtcNow;

                var result = await _salaryRepository.UpdateAsync(id, existingSalary);
                if (result)
                {
                    return Ok(new { message = "Cập nhật lương thành công" });
                }
                else
                {
                    return StatusCode(500, new { message = "Lỗi khi cập nhật lương" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating salary: {Id}", id);
                return StatusCode(500, new { message = "Lỗi khi cập nhật lương" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalary(string id, [FromQuery] bool force = false)
        {
            try
            {
                var existingSalary = await _salaryRepository.GetByIdAsync(id);
                if (existingSalary == null)
                {
                    return NotFound(new { message = "Không tìm thấy bản ghi lương" });
                }

                // Kiểm tra nếu lương đã được thanh toán thì không cho xóa
                if (existingSalary.IsPaid && !force)
                {
                    return BadRequest(new { message = "Không thể xóa lương đã được thanh toán" });
                }

                var result = await _salaryRepository.DeleteAsync(id);
                if (result)
                {
                    return Ok(new { message = "Xóa lương thành công" });
                }
                else
                {
                    return StatusCode(500, new { message = "Lỗi khi xóa lương" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting salary: {Id}", id);
                return StatusCode(500, new { message = "Lỗi khi xóa lương" });
            }
        }

        [HttpGet("department/{department}")]
        public async Task<IActionResult> GetSalariesByDepartment(string department, [FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var salaries = await _salaryRepository.GetByDepartmentAsync(department, year, month);
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salaries by department: {Department}", department);
                return StatusCode(500, new { message = "Lỗi khi lấy lương theo phòng ban" });
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetSalariesByDateRange([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var salaries = await _salaryRepository.GetByDateRangeAsync(year, month);
                return Ok(salaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting salaries by date range: {Year}/{Month}", year, month);
                return StatusCode(500, new { message = "Lỗi khi lấy lương theo thời gian" });
            }
        }
    }
}
