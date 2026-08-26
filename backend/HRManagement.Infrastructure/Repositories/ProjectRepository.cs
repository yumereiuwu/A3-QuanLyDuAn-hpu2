using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(HRManagementDbContext context, ILogger<ProjectRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Project?> GetByIdAsync(string id)
        {
            return await _context.Projects.FindAsync(id);
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _context.Projects
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status)
        {
            return await _context.Projects
                .Where(p => p.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetProjectsByTechnologyAsync(string technology)
        {
            return await _context.Projects
                .Where(p => p.Technology.Contains(technology))
                .ToListAsync();
        }

        public async Task<string> AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project.Id;
        }

        public async Task<bool> UpdateAsync(string id, Project project)
        {
            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null) return false;

            _context.Entry(existingProject).CurrentValues.SetValues(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Projects.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(string projectId)
        {
            return await _context.ProjectMembers
                .Where(pm => pm.ProjectId == projectId && pm.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetProjectTasksAsync(string projectId)
        {
            return await _context.ProjectTasks
                .Where(pt => pt.ProjectId == projectId)
                .ToListAsync();
        }
    }
}
