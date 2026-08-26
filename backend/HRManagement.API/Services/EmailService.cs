using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRManagement.API.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordEmailAsync(string employeeEmail, string employeeName, string password);
        Task<bool> SendSalaryEmailAsync(string employeeEmail, string employeeName, SalaryEmailData salaryData);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordEmailAsync(string employeeEmail, string employeeName, string password)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                _logger.LogInformation($"Attempting to send email to {employeeEmail}");
                _logger.LogInformation($"SMTP Server: {smtpServer}:{smtpPort}");
                _logger.LogInformation($"Username: {smtpUsername}");
                _logger.LogInformation($"SSL Enabled: {enableSsl}");

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = enableSsl;
                client.Timeout = 30000; // 30 seconds timeout

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(employeeEmail);
                message.Subject = "Thông tin tài khoản HR Management System";
                message.IsBodyHtml = true;
                message.Body = GenerateEmailBody(employeeName, employeeEmail, password);

                _logger.LogInformation($"Sending email...");
                await client.SendMailAsync(message);
                
                _logger.LogInformation($"✅ Email sent successfully to {employeeEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send email to {employeeEmail}: {ex.Message}");
                _logger.LogError($"Inner exception: {ex.InnerException?.Message}");
                return false;
            }
        }

        private string GenerateEmailBody(string employeeName, string employeeEmail, string password)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='background-color: #1976d2; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center;'>
                        <h1 style='margin: 0; font-size: 24px;'>🏢 HR Management System</h1>
                    </div>
                    
                    <div style='background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px;'>
                        <h2 style='color: #1976d2; margin-top: 0;'>Chào {employeeName}!</h2>
                        
                        <p style='font-size: 16px; line-height: 1.6; color: #333;'>
                            Tài khoản của bạn đã được tạo thành công trong hệ thống quản lý nhân sự.
                        </p>
                        
                        <div style='background-color: white; padding: 25px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #1976d2;'>
                            <h3 style='color: #333; margin-top: 0; font-size: 18px;'>🔑 Thông tin đăng nhập:</h3>
                            <p style='margin: 10px 0; font-size: 16px;'><strong>📧 Email:</strong> {employeeEmail}</p>
                            <p style='margin: 10px 0; font-size: 16px;'><strong>🔒 Mật khẩu:</strong> 
                                <span style='background-color: #e3f2fd; padding: 8px 12px; border-radius: 4px; font-family: monospace; font-weight: bold; color: #1976d2;'>{password}</span>
                            </p>
                        </div>
                        
                        <div style='background-color: #fff3cd; padding: 20px; border-radius: 8px; border-left: 4px solid #ffc107; margin: 25px 0;'>
                            <p style='margin: 0; font-weight: bold; color: #856404;'>⚠️ Lưu ý quan trọng:</p>
                            <p style='margin: 10px 0 0 0; color: #856404;'>Vui lòng đổi mật khẩu sau lần đăng nhập đầu tiên để bảo mật tài khoản.</p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6;'>
                            <p style='color: #6c757d; font-size: 14px; margin: 0;'>
                                Trân trọng,<br>
                                <strong style='color: #1976d2;'>HR Management Team</strong>
                            </p>
                        </div>
                    </div>
                </div>
            ";
        }

        public async Task<bool> SendSalaryEmailAsync(string employeeEmail, string employeeName, SalaryEmailData salaryData)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];
                var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                _logger.LogInformation($"Attempting to send salary email to {employeeEmail}");

                using var client = new SmtpClient(smtpServer, smtpPort);
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = enableSsl;
                client.Timeout = 30000; // 30 seconds timeout

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(employeeEmail);
                message.Subject = $"💰 Thông báo lương tháng {salaryData.Month}/{salaryData.Year} - {DateTime.Now:dd/MM/yyyy HH:mm}";
                message.IsBodyHtml = true;
                message.Body = GenerateSalaryEmailBody(employeeName, salaryData);

                _logger.LogInformation($"Sending salary email...");
                await client.SendMailAsync(message);
                
                _logger.LogInformation($"✅ Salary email sent successfully to {employeeEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Failed to send salary email to {employeeEmail}: {ex.Message}");
                return false;
            }
        }

        private string GenerateSalaryEmailBody(string employeeName, SalaryEmailData salaryData)
        {
            var statusText = salaryData.IsPaid ? "✅ Đã thanh toán" : "⏳ Chờ thanh toán";
            var statusColor = salaryData.IsPaid ? "#28a745" : "#ffc107";

            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <div style='background-color: #1976d2; color: white; padding: 20px; border-radius: 8px 8px 0 0; text-align: center;'>
                        <h1 style='margin: 0; font-size: 24px;'>💰 Thông báo lương</h1>
                        <p style='margin: 8px 0 0 0; font-size: 14px; opacity: 0.9;'>
                            📅 Gửi lúc: {DateTime.Now:dddd, dd/MM/yyyy 'lúc' HH:mm:ss}
                        </p>
                    </div>
                    
                    <div style='background-color: #f8f9fa; padding: 30px; border-radius: 0 0 8px 8px;'>
                        <h2 style='color: #1976d2; margin-top: 0;'>Xin chào {employeeName}!</h2>
                        
                        <p style='font-size: 16px; line-height: 1.6; color: #333;'>
                            Đây là thông tin lương của bạn cho tháng {salaryData.Month}/{salaryData.Year}:
                        </p>
                        
                        <div style='background-color: white; padding: 25px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #1976d2;'>
                            <h3 style='color: #333; margin-top: 0; font-size: 18px;'>📊 Chi tiết lương:</h3>
                            
                            <div style='display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee;'>
                                <span style='color: #666;'>Lương cơ bản:</span>
                                <span style='font-weight: bold; color: #2e7d32;'>{salaryData.BasicSalary:N0} ₫</span>
                            </div>
                            
                            <div style='display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee;'>
                                <span style='color: #666;'>Phụ cấp:</span>
                                <span style='font-weight: bold; color: #1976d2;'>{salaryData.Allowance:N0} ₫</span>
                            </div>
                            
                            <div style='display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee;'>
                                <span style='color: #666;'>Thưởng:</span>
                                <span style='font-weight: bold; color: #f57c00;'>{salaryData.Bonus:N0} ₫</span>
                            </div>
                            
                            <div style='display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee;'>
                                <span style='color: #666;'>Overtime:</span>
                                <span style='font-weight: bold; color: #7b1fa2;'>{salaryData.OvertimePay:N0} ₫</span>
                            </div>
                            
                            <div style='display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee;'>
                                <span style='color: #666;'>Khấu trừ:</span>
                                <span style='font-weight: bold; color: #d32f2f;'>{salaryData.Deductions:N0} ₫</span>
                            </div>
                            
                            <div style='display: flex; justify-content: space-between; margin: 20px 0 10px 0; padding: 15px 0; border-top: 2px solid #1976d2; background-color: #e3f2fd; border-radius: 4px;'>
                                <span style='font-size: 18px; font-weight: bold; color: #1976d2;'>Lương thực lĩnh:</span>
                                <span style='font-size: 20px; font-weight: bold; color: #2e7d32;'>{salaryData.NetSalary:N0} ₫</span>
                            </div>
                        </div>
                        
                        <div style='background-color: white; padding: 20px; border-radius: 8px; margin: 25px 0;'>
                            <h3 style='color: #333; margin-top: 0; font-size: 16px;'>📋 Thông tin khác:</h3>
                            <p style='margin: 8px 0; color: #666;'><strong>Tháng/Năm:</strong> {salaryData.Month}/{salaryData.Year}</p>
                            <p style='margin: 8px 0; color: #666;'><strong>Phòng ban:</strong> {salaryData.Department}</p>
                            <p style='margin: 8px 0; color: #666;'><strong>Trạng thái:</strong> 
                                <span style='color: {statusColor}; font-weight: bold;'>{statusText}</span>
                            </p>
                        </div>
                        
                        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 25px 0; border-left: 4px solid #17a2b8;'>
                            <h3 style='color: #333; margin-top: 0; font-size: 16px;'>📅 Thông tin gửi email:</h3>
                            <p style='margin: 8px 0; color: #666;'><strong>Ngày gửi:</strong> {DateTime.Now:dd/MM/yyyy}</p>
                            <p style='margin: 8px 0; color: #666;'><strong>Giờ gửi:</strong> {DateTime.Now:HH:mm:ss}</p>
                            <p style='margin: 8px 0; color: #666;'><strong>Thời gian:</strong> {DateTime.Now:dddd, dd/MM/yyyy 'lúc' HH:mm:ss}</p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #dee2e6;'>
                            <p style='color: #6c757d; font-size: 14px; margin: 0;'>
                                Trân trọng,<br>
                                <strong style='color: #1976d2;'>Phòng Nhân sự</strong><br>
                                <span style='font-size: 12px; color: #999;'>Hệ thống Quản lý Nhân sự</span>
                            </p>
                        </div>
                    </div>
                </div>
            ";
        }
    }

    public class SalaryEmailData
    {
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
