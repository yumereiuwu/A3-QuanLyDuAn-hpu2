using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<SkillRepository> _logger;

        public SkillRepository(HRManagementDbContext context, ILogger<SkillRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Skill?> GetByIdAsync(string id)
        {
            return await _context.Skills.FindAsync(id);
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            return await _context.Skills.ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetByUserIdAsync(string userId)
        {
            return await _context.Skills
                .Where(s => s.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetByCategoryAsync(string category)
        {
            return await _context.Skills
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Skill>> GetByLevelAsync(string level)
        {
            return await _context.Skills
                .Where(s => s.Level == level)
                .ToListAsync();
        }

        public async Task<string> AddAsync(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return skill.Id;
        }

        public async Task<bool> UpdateAsync(string id, Skill skill)
        {
            var existingSkill = await _context.Skills.FindAsync(id);
            if (existingSkill == null) return false;

            _context.Entry(existingSkill).CurrentValues.SetValues(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return false;

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Skills.AnyAsync(s => s.Id == id);
        }
    }
}
