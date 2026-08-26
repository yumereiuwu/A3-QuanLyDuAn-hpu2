using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface IPerformanceReviewRepository
    {
        Task<PerformanceReview?> GetByIdAsync(string id);
        Task<IEnumerable<PerformanceReview>> GetAllAsync();
        Task<IEnumerable<PerformanceReview>> GetByUserIdAsync(string userId);
        Task<PerformanceReview?> GetByUserIdAndPeriodAsync(string userId, int year, int quarter);
        Task<IEnumerable<PerformanceReview>> GetByPeriodAsync(int year, int quarter);
        Task<IEnumerable<PerformanceReview>> GetByReviewerIdAsync(string reviewerId);
        Task<IEnumerable<PerformanceReview>> GetPendingReviewsAsync();
        Task<string> AddAsync(PerformanceReview review);
        Task<bool> UpdateAsync(string id, PerformanceReview review);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
