using HRManagement.Core.Entities;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRManagement.Infrastructure.Repositories
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly HRManagementDbContext _context;
        private readonly ILogger<CertificateRepository> _logger;

        public CertificateRepository(HRManagementDbContext context, ILogger<CertificateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Certificate?> GetByIdAsync(string id)
        {
            return await _context.Certificates.FindAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetAllAsync()
        {
            return await _context.Certificates.ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetByUserIdAsync(string userId)
        {
            return await _context.Certificates
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetByIssuerAsync(string issuer)
        {
            return await _context.Certificates
                .Where(c => c.Issuer == issuer)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetExpiringCertificatesAsync(DateTime expiryDate)
        {
            return await _context.Certificates
                .Where(c => c.ExpiryDate <= expiryDate)
                .ToListAsync();
        }

        public async Task<string> AddAsync(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return certificate.Id;
        }

        public async Task<bool> UpdateAsync(string id, Certificate certificate)
        {
            var existingCertificate = await _context.Certificates.FindAsync(id);
            if (existingCertificate == null) return false;

            _context.Entry(existingCertificate).CurrentValues.SetValues(certificate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return false;

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Certificates.AnyAsync(c => c.Id == id);
        }
    }
}
