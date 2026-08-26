using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagement.Core.Entities
{
    [Table("Timesheets")]
    public class Timesheet
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("Project")]
        public string? ProjectId { get; set; }

        [ForeignKey("ProjectTask")]
        public string? TaskId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal HoursWorked { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Project? Project { get; set; }
        public virtual ProjectTask? ProjectTask { get; set; }
        public virtual User? ApprovedByUser { get; set; }
    }

    [Table("Attendance")]
    public class Attendance
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? CheckInPhoto { get; set; } // Base64 hoặc URL ảnh check-in

        [Column(TypeName = "nvarchar(max)")]
        public string? CheckOutPhoto { get; set; } // Base64 hoặc URL ảnh check-out

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Present, Absent, Late, Half Day

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }

    [Table("LeaveRequests")]
    public class LeaveRequest
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LeaveType { get; set; } = string.Empty; // Annual, Sick, Personal, Maternity, Paternity

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int TotalDays { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovedAt { get; set; }

        [ForeignKey("ApprovedByUser")]
        public string? ApprovedByUserId { get; set; }

        [MaxLength(1000)]
        public string? RejectionReason { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual User? ApprovedByUser { get; set; }
    }
}
