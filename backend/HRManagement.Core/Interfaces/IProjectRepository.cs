using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(string id);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
        Task<IEnumerable<Project>> GetProjectsByStatusAsync(string status);
        Task<IEnumerable<Project>> GetProjectsByTechnologyAsync(string technology);
        Task<string> AddAsync(Project project);
        Task<bool> UpdateAsync(string id, Project project);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(string projectId);
        Task<IEnumerable<ProjectTask>> GetProjectTasksAsync(string projectId);
    }
}
