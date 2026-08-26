using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagement.Core.Entities
{
    [Table("Certificates")]
    public class Certificate
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string Issuer { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserCertificate> UserCertificates { get; set; } = new List<UserCertificate>();
    }

    [Table("UserCertificates")]
    public class UserCertificate
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Certificate")]
        public string CertificateId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? CertificateNumber { get; set; }

        [Required]
        public DateTime AcquiredDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [MaxLength(500)]
        public string? FilePath { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Certificate Certificate { get; set; } = null!;
    }
}
