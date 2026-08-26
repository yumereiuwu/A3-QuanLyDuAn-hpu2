using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagement.Core.Entities
{
    [Table("Skills")]
    public class Skill
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty; // Beginner, Intermediate, Expert

        [Required]
        [MaxLength(100)]
        public string UserId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }

    [Table("UserSkills")]
    public class UserSkill
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Skill")]
        public string SkillId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty; // Beginner, Intermediate, Expert

        [Range(0, 100)]
        public int ProficiencyPercentage { get; set; } = 0;

        public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Skill Skill { get; set; } = null!;
    }
}
