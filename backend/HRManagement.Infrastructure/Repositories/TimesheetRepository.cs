using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<TimesheetRepository> _logger;

        public TimesheetRepository(HRManagementDbContext context, ILogger<TimesheetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Timesheet?> GetByIdAsync(string id)
        {
            return await _context.Timesheets.FindAsync(id);
        }

        public async Task<IEnumerable<Timesheet>> GetAllAsync()
        {
            return await _context.Timesheets.ToListAsync();
        }

        public async Task<IEnumerable<Timesheet>> GetByUserIdAsync(string userId)
        {
            return await _context.Timesheets
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Timesheet>> GetByProjectIdAsync(string projectId)
        {
            return await _context.Timesheets
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Timesheet>> GetByDateRangeAsync(string userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Timesheets
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();
        }

        public async Task<string> AddAsync(Timesheet timesheet)
        {
            _context.Timesheets.Add(timesheet);
            await _context.SaveChangesAsync();
            return timesheet.Id;
        }

        public async Task<bool> UpdateAsync(string id, Timesheet timesheet)
        {
            var existingTimesheet = await _context.Timesheets.FindAsync(id);
            if (existingTimesheet == null) return false;

            _context.Entry(existingTimesheet).CurrentValues.SetValues(timesheet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null) return false;

            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Timesheets.AnyAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Attendance>> GetAttendanceByUserIdAsync(string userId)
        {
            return await _context.Attendance
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByUserIdAsync(string userId)
        {
            return await _context.LeaveRequests
                .Where(lr => lr.UserId == userId)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByDateAsync(string userId, DateTime date)
        {
            return await _context.Attendance
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == date.Date);
        }
    }
}
