using Microsoft.EntityFrameworkCore;
using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;

namespace HRManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HRManagementDbContext _context;

        public UserRepository(HRManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .Include(u => u.ProjectMembers)
                    .ThenInclude(pm => pm.Project)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByDepartmentAsync(string department)
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .Where(u => u.Department == department && u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(string role)
        {
            return await _context.Users
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .Include(u => u.UserCertificates)
                    .ThenInclude(uc => uc.Certificate)
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<string> AddAsync(User user)
        {
            user.Id = Guid.NewGuid().ToString();
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> UpdateAsync(string id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return false;

            user.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // Soft delete + release unique identifiers so the email/username can be reused
            // Keep a traceable value by appending a suffix
            var suffix = $"__deleted_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            if (!string.IsNullOrEmpty(user.Email))
            {
                user.Email = user.Email + suffix;
            }
            if (!string.IsNullOrEmpty(user.Username))
            {
                user.Username = user.Username + suffix;
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            // Case-insensitive check to match DB unique index behavior
            var normalized = email.Trim().ToLower();
            return !await _context.Users.AnyAsync(u => u.Email.ToLower() == normalized);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            var normalized = username.Trim().ToLower();
            return !await _context.Users.AnyAsync(u => u.Username.ToLower() == normalized);
        }

    }
}