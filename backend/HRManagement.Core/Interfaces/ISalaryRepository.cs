using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface ISalaryRepository
    {
        Task<Salary?> GetByIdAsync(string id);
        Task<IEnumerable<Salary>> GetAllAsync();
        Task<IEnumerable<Salary>> GetByUserIdAsync(string userId);
        Task<Salary?> GetByUserIdAndMonthAsync(string userId, int year, int month);
        Task<IEnumerable<Salary>> GetByDepartmentAsync(string department, int year, int month);
        Task<IEnumerable<Salary>> GetByDateRangeAsync(int year, int month);
        Task<string> AddAsync(Salary salary);
        Task<bool> UpdateAsync(string id, Salary salary);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
