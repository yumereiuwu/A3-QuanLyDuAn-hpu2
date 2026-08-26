using Microsoft.AspNetCore.Mvc;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;

namespace HRManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowReactApp")]
    public class ReportsController : ControllerBase
    {
        private readonly HRManagementDbContext _context;

        public ReportsController(HRManagementDbContext context)
        {
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working!", timestamp = DateTime.Now });
        }

        [HttpGet("employee-stats")]
        public async Task<IActionResult> GetEmployeeStats()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var totalEmployees = await _context.Users
                .Where(u => u.Role != "Admin" && u.IsActive)
                .CountAsync();
            var newEmployeesThisMonth = await _context.Users
                .Where(u => u.Role != "Admin" && u.IsActive && u.CreatedAt.Month == currentMonth && u.CreatedAt.Year == currentYear)
                .CountAsync();
            var resignedThisMonth = await _context.Users
                .Where(u => u.Role != "Admin" && !u.IsActive && u.UpdatedAt.Month == currentMonth && u.UpdatedAt.Year == currentYear)
                .CountAsync();

            var resignationRate = totalEmployees > 0 ? (double)resignedThisMonth / totalEmployees * 100 : 0;

            return Ok(new
            {
                totalEmployees,
                newEmployeesThisMonth,
                resignedThisMonth,
                resignationRate = Math.Round(resignationRate, 1)
            });
        }

        [HttpGet("salary-stats")]
        public async Task<IActionResult> GetSalaryStats()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var currentMonthSalaries = await _context.Salaries
                .Where(s => s.Year == currentYear && s.Month == currentMonth)
                .ToListAsync();

            var totalSalaryFund = currentMonthSalaries.Sum(s => s.NetSalary);
            var averageSalary = currentMonthSalaries.Any() ? currentMonthSalaries.Average(s => s.NetSalary) : 0;
            var totalBonus = currentMonthSalaries.Sum(s => s.Bonus);

            // So sánh với tháng trước
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;
            
            var lastMonthSalaries = await _context.Salaries
                .Where(s => s.Year == lastMonthYear && s.Month == lastMonth)
                .ToListAsync();

            var lastMonthTotal = lastMonthSalaries.Sum(s => s.NetSalary);
            var growthRate = lastMonthTotal > 0 ? (totalSalaryFund - lastMonthTotal) / lastMonthTotal * 100 : 0;

            return Ok(new
            {
                totalSalaryFund,
                averageSalary = Math.Round(averageSalary, 0),
                totalBonus,
                growthRate = Math.Round(growthRate, 1)
            });
        }

        [HttpGet("performance-stats")]
        public async Task<IActionResult> GetPerformanceStats()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Tính giờ làm trung bình từ Timesheet
            var timesheets = await _context.Timesheets
                .Where(t => t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .ToListAsync();

            var avgHoursPerDay = timesheets.Any() ? timesheets.Average(t => t.HoursWorked) : 0;

            // Tính tỷ lệ đi làm đúng giờ từ Attendance
            var attendance = await _context.Attendance
                .Where(a => a.Date.Month == currentMonth && a.Date.Year == currentYear)
                .ToListAsync();

            var onTimeRate = attendance.Any() ? 
                (double)attendance.Count(a => a.Status == "Present" && a.CheckInTime.HasValue && a.CheckInTime.Value.Hour <= 9) / attendance.Count * 100 : 0;

            // Tính overtime
            var totalOvertime = timesheets.Sum(t => Math.Max(0, t.HoursWorked - 8));

            // Đánh giá hiệu suất trung bình
            var performanceReviews = await _context.PerformanceReviews
                .Where(p => p.Year == currentYear)
                .ToListAsync();

            var avgPerformance = performanceReviews.Any() ? performanceReviews.Average(p => p.OverallRating) : 0;

            return Ok(new
            {
                avgHoursPerDay = Math.Round(avgHoursPerDay, 1),
                onTimeRate = Math.Round(onTimeRate, 1),
                totalOvertime = Math.Round(totalOvertime, 0),
                avgPerformance = Math.Round(avgPerformance, 1)
            });
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetTrends()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Tăng trưởng nhân sự (so với tháng trước)
            var lastMonth = currentMonth == 1 ? 12 : currentMonth - 1;
            var lastMonthYear = currentMonth == 1 ? currentYear - 1 : currentYear;

            var currentMonthEmployees = await _context.Users
                .Where(u => u.CreatedAt.Month <= currentMonth && u.CreatedAt.Year == currentYear)
                .CountAsync();

            var lastMonthEmployees = await _context.Users
                .Where(u => u.CreatedAt.Month <= lastMonth && u.CreatedAt.Year == lastMonthYear)
                .CountAsync();

            var personnelGrowth = lastMonthEmployees > 0 ? 
                (double)(currentMonthEmployees - lastMonthEmployees) / lastMonthEmployees * 100 : 0;

            // Tăng trưởng năng suất (dựa trên giờ làm)
            var currentMonthTimesheets = await _context.Timesheets
                .Where(t => t.Date.Month == currentMonth && t.Date.Year == currentYear)
                .ToListAsync();
            var currentMonthHours = currentMonthTimesheets.Sum(t => t.HoursWorked);

            var lastMonthTimesheets = await _context.Timesheets
                .Where(t => t.Date.Month == lastMonth && t.Date.Year == lastMonthYear)
                .ToListAsync();
            var lastMonthHours = lastMonthTimesheets.Sum(t => t.HoursWorked);

            var productivityGrowth = lastMonthHours > 0 ? 
                (currentMonthHours - lastMonthHours) / lastMonthHours * 100 : 0;

            // Giảm chi phí vận hành (dựa trên lương)
            var currentMonthSalaries = await _context.Salaries
                .Where(s => s.Year == currentYear && s.Month == currentMonth)
                .ToListAsync();
            var currentMonthSalary = currentMonthSalaries.Sum(s => s.NetSalary);

            var lastMonthSalaries = await _context.Salaries
                .Where(s => s.Year == lastMonthYear && s.Month == lastMonth)
                .ToListAsync();
            var lastMonthSalary = lastMonthSalaries.Sum(s => s.NetSalary);

            var costReduction = lastMonthSalary > 0 ? 
                (lastMonthSalary - currentMonthSalary) / lastMonthSalary * 100 : 0;

            // Tăng doanh thu/người (giả định dựa trên hiệu suất)
            var revenuePerPersonGrowth = productivityGrowth * 1.5m; // Giả định

            return Ok(new
            {
                personnelGrowth = Math.Round(personnelGrowth, 1),
                productivityGrowth = Math.Round(productivityGrowth, 1),
                costReduction = Math.Round(costReduction, 1),
                revenuePerPersonGrowth = Math.Round(revenuePerPersonGrowth, 1)
            });
        }

        // New: Employee headcount by month for a year (active employees at the end of each month)
        [HttpGet("employee-monthly")]
        public async Task<IActionResult> GetEmployeeMonthly([FromQuery] int? year)
        {
            var targetYear = year ?? DateTime.Now.Year;

            // Load minimal fields once
            var users = await _context.Users
                .Select(u => new { u.CreatedAt, u.UpdatedAt, u.IsActive })
                .ToListAsync();

            var result = new List<object>();

            for (int m = 1; m <= 12; m++)
            {
                var monthEnd = new DateTime(targetYear, m, DateTime.DaysInMonth(targetYear, m), 23, 59, 59);

                // Employee is counted if:
                // - Joined on or before monthEnd, AND
                // - Either still active now, or if deactivated then the deactivation (UpdatedAt) is AFTER monthEnd
                var headcount = users.Count(u =>
                    u.CreatedAt <= monthEnd &&
                    (u.IsActive || u.UpdatedAt > monthEnd)
                );

                result.Add(new { month = m, count = headcount });
            }

            return Ok(result);
        }

        // New: Resignation rate for a year
        [HttpGet("resignation-rate-year")]
        public async Task<IActionResult> GetResignationRateYear([FromQuery] int? year)
        {
            var targetYear = year ?? DateTime.Now.Year;
            var totalEmployees = await _context.Users.CountAsync();
            var resigned = await _context.Users
                .Where(u => !u.IsActive && u.UpdatedAt.Year == targetYear)
                .CountAsync();

            var rate = totalEmployees > 0 ? Math.Round((double)resigned / totalEmployees * 100, 1) : 0;
            return Ok(new { year = targetYear, total = totalEmployees, resigned, rate });
        }

        // New: Department counts
        [HttpGet("department-counts")]
        public async Task<IActionResult> GetDepartmentCounts()
        {
            var data = await _context.Users
                .Where(u => u.IsActive && u.Role != "Admin")
                .GroupBy(u => u.Department)
                .Select(g => new { department = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            return Ok(data);
        }

        // New: Combined employees & total salary for recent N months (default 6)
        [HttpGet("employees-salary-combined")]
        public async Task<IActionResult> GetEmployeesSalaryCombined([FromQuery] int months = 6)
        {
            months = Math.Clamp(months, 1, 12);
            var now = DateTime.Now;

            // Preload counts to avoid per-iteration DB calls
            var employeeCount = await _context.Users.CountAsync(u => u.IsActive);
            var salaries = await _context.Salaries
                .Select(s => new { s.Year, s.Month, s.NetSalary })
                .ToListAsync();

            var result = new List<object>();
            for (int i = months - 1; i >= 0; i--)
            {
                var dt = now.AddMonths(-i);
                var y = dt.Year;
                var m = dt.Month;
                var salaryTotal = salaries
                    .Where(s => s.Year == y && s.Month == m)
                    .Sum(s => s.NetSalary);
                result.Add(new { year = y, month = m, employees = employeeCount, totalSalary = salaryTotal });
            }

            return Ok(result);
        }

        // New: Top N highest salaries for current month with user info
        [HttpGet("top-salaries")]
        public async Task<IActionResult> GetTopSalaries([FromQuery] int limit = 5)
        {
            var now = DateTime.Now;
            limit = Math.Clamp(limit, 1, 50);

            var data = await _context.Salaries
                .Include(s => s.User)
                .Where(s => s.Year == now.Year && s.Month == now.Month)
                .OrderByDescending(s => s.NetSalary)
                .Take(limit)
                .Select(s => new { name = s.User.FullName, department = s.Department, netSalary = s.NetSalary })
                .ToListAsync();

            return Ok(data);
        }

        // New: Rewards & Discipline summary derived from Salary bonuses/deductions
        [HttpGet("rewards-discipline")]
        public async Task<IActionResult> GetRewardsDiscipline([FromQuery] int limit = 10)
        {
            limit = Math.Clamp(limit, 1, 100);
            var now = DateTime.Now.AddMonths(-6); // last 6 months

            var startYear = now.Year;
            var startMonth = now.Month;
            var rewards = await _context.Salaries
                .Include(s => s.User)
                .Where(s => s.Bonus > 0)
                .ToListAsync();
            rewards = rewards
                .Where(s => (s.Year > startYear) || (s.Year == startYear && s.Month >= startMonth))
                .OrderByDescending(s => s.Bonus)
                .Take(limit)
                .ToList();

            var discipline = await _context.Salaries
                .Include(s => s.User)
                .Where(s => s.Deductions > 0)
                .ToListAsync();
            discipline = discipline
                .Where(s => (s.Year > startYear) || (s.Year == startYear && s.Month >= startMonth))
                .OrderByDescending(s => s.Deductions)
                .Take(limit)
                .ToList();

            return Ok(new { rewards, discipline });
        }
    }
}
