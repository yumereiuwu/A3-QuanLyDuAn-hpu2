using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagement.Core.Entities
{
    [Table("Salaries")]
    public class Salary
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal BasicSalary { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Allowance { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Bonus { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal OvertimePay { get; set; }

        [Column(TypeName = "decimal(15,2)")]
        public decimal Deductions { get; set; }

        [Required]
        [Column(TypeName = "decimal(15,2)")]
        public decimal NetSalary { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }

    [Table("PerformanceReviews")]
    public class PerformanceReview
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Reviewer")]
        public string ReviewerId { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Quarter { get; set; }

        [Required]
        [Range(1, 5)]
        public int TechnicalSkills { get; set; } // 1-5 rating

        [Required]
        [Range(1, 5)]
        public int Communication { get; set; } // 1-5 rating

        [Required]
        [Range(1, 5)]
        public int Teamwork { get; set; } // 1-5 rating

        [Required]
        [Range(1, 5)]
        public int ProblemSolving { get; set; } // 1-5 rating

        [Required]
        [Range(1, 5)]
        public int Productivity { get; set; } // 1-5 rating

        [Required]
        [Range(1, 5)]
        public int OverallRating { get; set; } // 1-5 rating

        [MaxLength(1000)]
        public string? Strengths { get; set; }

        [MaxLength(1000)]
        public string? AreasForImprovement { get; set; }

        [MaxLength(1000)]
        public string? Goals { get; set; }

        [MaxLength(1000)]
        public string? Comments { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled

        public DateTime? ReviewDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual User Reviewer { get; set; } = null!;
    }
}
