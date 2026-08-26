using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class SalaryRepository : ISalaryRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<SalaryRepository> _logger;

        public SalaryRepository(HRManagementDbContext context, ILogger<SalaryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Salary?> GetByIdAsync(string id)
        {
            return await _context.Salaries.FindAsync(id);
        }

        public async Task<IEnumerable<Salary>> GetAllAsync()
        {
            return await _context.Salaries.ToListAsync();
        }

        public async Task<IEnumerable<Salary>> GetByUserIdAsync(string userId)
        {
            return await _context.Salaries
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<Salary?> GetByUserIdAndMonthAsync(string userId, int year, int month)
        {
            return await _context.Salaries
                .FirstOrDefaultAsync(s => s.UserId == userId && s.Year == year && s.Month == month);
        }

        public async Task<IEnumerable<Salary>> GetByDepartmentAsync(string department, int year, int month)
        {
            return await _context.Salaries
                .Where(s => s.Department == department && s.Year == year && s.Month == month)
                .ToListAsync();
        }

        public async Task<IEnumerable<Salary>> GetByDateRangeAsync(int year, int month)
        {
            return await _context.Salaries
                .Where(s => s.Year == year && s.Month == month)
                .ToListAsync();
        }

        public async Task<string> AddAsync(Salary salary)
        {
            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();
            return salary.Id;
        }

        public async Task<bool> UpdateAsync(string id, Salary salary)
        {
            var existingSalary = await _context.Salaries.FindAsync(id);
            if (existingSalary == null) return false;

            _context.Entry(existingSalary).CurrentValues.SetValues(salary);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null) return false;

            _context.Salaries.Remove(salary);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Salaries.AnyAsync(s => s.Id == id);
        }
    }
}
