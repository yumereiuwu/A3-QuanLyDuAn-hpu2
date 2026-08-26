using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class PerformanceReviewRepository : IPerformanceReviewRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<PerformanceReviewRepository> _logger;

        public PerformanceReviewRepository(HRManagementDbContext context, ILogger<PerformanceReviewRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PerformanceReview?> GetByIdAsync(string id)
        {
            return await _context.PerformanceReviews.FindAsync(id);
        }

        public async Task<IEnumerable<PerformanceReview>> GetAllAsync()
        {
            return await _context.PerformanceReviews.ToListAsync();
        }

        public async Task<IEnumerable<PerformanceReview>> GetByUserIdAsync(string userId)
        {
            return await _context.PerformanceReviews
                .Where(pr => pr.UserId == userId)
                .ToListAsync();
        }

        public async Task<PerformanceReview?> GetByUserIdAndPeriodAsync(string userId, int year, int quarter)
        {
            return await _context.PerformanceReviews
                .FirstOrDefaultAsync(pr => pr.UserId == userId && pr.Year == year && pr.Quarter == quarter);
        }

        public async Task<IEnumerable<PerformanceReview>> GetByReviewerIdAsync(string reviewerId)
        {
            return await _context.PerformanceReviews
                .Where(pr => pr.ReviewerId == reviewerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PerformanceReview>> GetByPeriodAsync(int year, int quarter)
        {
            return await _context.PerformanceReviews
                .Where(pr => pr.Year == year && pr.Quarter == quarter)
                .ToListAsync();
        }

        public async Task<IEnumerable<PerformanceReview>> GetPendingReviewsAsync()
        {
            return await _context.PerformanceReviews
                .Where(pr => pr.Status == "Pending")
                .ToListAsync();
        }

        public async Task<string> AddAsync(PerformanceReview review)
        {
            _context.PerformanceReviews.Add(review);
            await _context.SaveChangesAsync();
            return review.Id;
        }

        public async Task<bool> UpdateAsync(string id, PerformanceReview review)
        {
            var existingReview = await _context.PerformanceReviews.FindAsync(id);
            if (existingReview == null) return false;

            _context.Entry(existingReview).CurrentValues.SetValues(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var review = await _context.PerformanceReviews.FindAsync(id);
            if (review == null) return false;

            _context.PerformanceReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.PerformanceReviews.AnyAsync(pr => pr.Id == id);
        }
    }
}
