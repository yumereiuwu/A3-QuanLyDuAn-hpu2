using Microsoft.AspNetCore.Mvc;
using HRManagement.API.Services;

namespace HRManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("send-password")]
        public async Task<IActionResult> SendPasswordEmail([FromBody] SendPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Email, Name và Password không được để trống" });
                }

                var result = await _emailService.SendPasswordEmailAsync(request.Email, request.Name, request.Password);
                
                if (result)
                {
                    return Ok(new { 
                        success = true, 
                        message = $"Email đã được gửi thành công đến {request.Email}" 
                    });
                }
                else
                {
                    return StatusCode(500, new { 
                        success = false, 
                        message = "Không thể gửi email. Vui lòng thử lại sau." 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendPasswordEmail endpoint");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi server khi gửi email" 
                });
            }
        }

        [HttpPost("send-salary")]
        public async Task<IActionResult> SendSalaryEmail([FromBody] SendSalaryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new { message = "Email và Name không được để trống" });
                }

                var salaryData = new SalaryEmailData
                {
                    Month = request.Month,
                    Year = request.Year,
                    Department = request.Department,
                    BasicSalary = request.BasicSalary,
                    Allowance = request.Allowance,
                    Bonus = request.Bonus,
                    OvertimePay = request.OvertimePay,
                    Deductions = request.Deductions,
                    NetSalary = request.NetSalary,
                    IsPaid = request.IsPaid
                };

                var result = await _emailService.SendSalaryEmailAsync(request.Email, request.Name, salaryData);
                
                if (result)
                {
                    return Ok(new { 
                        success = true, 
                        message = $"Email thông báo lương đã được gửi thành công đến {request.Email}" 
                    });
                }
                else
                {
                    return StatusCode(500, new { 
                        success = false, 
                        message = "Không thể gửi email. Vui lòng thử lại sau." 
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendSalaryEmail endpoint");
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi server khi gửi email" 
                });
            }
        }
    }

    public class SendPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SendSalaryRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public string Department { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Bonus { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsPaid { get; set; }
    }
}
