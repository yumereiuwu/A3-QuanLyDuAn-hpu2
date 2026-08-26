using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface ITimesheetRepository
    {
        Task<Timesheet?> GetByIdAsync(string id);
        Task<IEnumerable<Timesheet>> GetAllAsync();
        Task<IEnumerable<Timesheet>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Timesheet>> GetByProjectIdAsync(string projectId);
        Task<IEnumerable<Timesheet>> GetByDateRangeAsync(string userId, DateTime startDate, DateTime endDate);
        Task<string> AddAsync(Timesheet timesheet);
        Task<bool> UpdateAsync(string id, Timesheet timesheet);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<IEnumerable<Attendance>> GetAttendanceByUserIdAsync(string userId);
        Task<IEnumerable<LeaveRequest>> GetLeaveRequestsByUserIdAsync(string userId);
        Task<Attendance?> GetAttendanceByDateAsync(string userId, DateTime date);
    }
}
