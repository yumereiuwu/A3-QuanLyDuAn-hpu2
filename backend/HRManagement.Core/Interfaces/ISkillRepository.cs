using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface ISkillRepository
    {
        Task<Skill?> GetByIdAsync(string id);
        Task<IEnumerable<Skill>> GetAllAsync();
        Task<IEnumerable<Skill>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Skill>> GetByCategoryAsync(string category);
        Task<IEnumerable<Skill>> GetByLevelAsync(string level);
        Task<string> AddAsync(Skill skill);
        Task<bool> UpdateAsync(string id, Skill skill);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
