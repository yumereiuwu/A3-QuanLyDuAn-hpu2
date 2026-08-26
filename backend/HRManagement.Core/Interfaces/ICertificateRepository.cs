using HRManagement.Core.Entities;

namespace HRManagement.Core.Interfaces
{
    public interface ICertificateRepository
    {
        Task<Certificate?> GetByIdAsync(string id);
        Task<IEnumerable<Certificate>> GetAllAsync();
        Task<IEnumerable<Certificate>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Certificate>> GetByIssuerAsync(string issuer);
        Task<IEnumerable<Certificate>> GetExpiringCertificatesAsync(DateTime expiryDate);
        Task<string> AddAsync(Certificate certificate);
        Task<bool> UpdateAsync(string id, Certificate certificate);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
