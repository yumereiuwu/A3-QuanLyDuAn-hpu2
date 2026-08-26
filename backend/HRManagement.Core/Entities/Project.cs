using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagement.Core.Entities
{
    [Table("Projects")]
    public class Project
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // Active, Completed, On Hold, Cancelled

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(255)]
        public string? Client { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Technology { get; set; } = "[]"; // JSON array of technologies

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
        public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
    }

    [Table("ProjectMembers")]
    public class ProjectMember
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Project")]
        public string ProjectId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty; // Dev, Tester, PM, Designer, etc.

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public DateTime? LeaveDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Project Project { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

    [Table("ProjectTasks")]
    public class ProjectTask
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Project")]
        public string ProjectId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("AssignedToUser")]
        public string AssignedToUserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // To Do, In Progress, Review, Done

        [Required]
        [MaxLength(50)]
        public string Priority { get; set; } = string.Empty; // Low, Medium, High, Critical

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Project Project { get; set; } = null!;
        public virtual User AssignedToUser { get; set; } = null!;
        public virtual ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();
    }
}
