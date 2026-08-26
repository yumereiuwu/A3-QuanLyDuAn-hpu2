using Microsoft.EntityFrameworkCore;
using HRManagement.Core.Entities;

namespace HRManagement.Infrastructure.Data
{
    public class HRManagementDbContext : DbContext
    {
        public HRManagementDbContext(DbContextOptions<HRManagementDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<UserCertificate> UserCertificates { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<Timesheet> Timesheets { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.UserSkills)
                .WithOne(us => us.User)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserCertificates)
                .WithOne(uc => uc.User)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ProjectMembers)
                .WithOne(pm => pm.User)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Timesheets)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Attendance)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.LeaveRequests)
                .WithOne(lr => lr.User)
                .HasForeignKey(lr => lr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Salaries)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.PerformanceReviews)
                .WithOne(pr => pr.User)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Skill relationships
            modelBuilder.Entity<Skill>()
                .HasMany(s => s.UserSkills)
                .WithOne(us => us.Skill)
                .HasForeignKey(us => us.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Certificate relationships
            modelBuilder.Entity<Certificate>()
                .HasMany(c => c.UserCertificates)
                .WithOne(uc => uc.Certificate)
                .HasForeignKey(uc => uc.CertificateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Project relationships
            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMembers)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectTasks)
                .WithOne(pt => pt.Project)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Timesheets)
                .WithOne(t => t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure ProjectTask relationships
            modelBuilder.Entity<ProjectTask>()
                .HasOne(pt => pt.AssignedToUser)
                .WithMany()
                .HasForeignKey(pt => pt.AssignedToUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTask>()
                .HasMany(pt => pt.Timesheets)
                .WithOne(t => t.ProjectTask)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Timesheet relationships
            modelBuilder.Entity<Timesheet>()
                .HasOne(t => t.ApprovedByUser)
                .WithMany()
                .HasForeignKey(t => t.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure LeaveRequest relationships
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.ApprovedByUser)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure PerformanceReview relationships
            modelBuilder.Entity<PerformanceReview>()
                .HasOne(pr => pr.Reviewer)
                .WithMany()
                .HasForeignKey(pr => pr.ReviewerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure indexes for performance
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.UserId, a.Date });

            modelBuilder.Entity<Timesheet>()
                .HasIndex(t => new { t.UserId, t.Date });

            modelBuilder.Entity<Salary>()
                .HasIndex(s => new { s.UserId, s.Year, s.Month });

            modelBuilder.Entity<LeaveRequest>()
                .HasIndex(lr => lr.UserId);

            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => pm.UserId);

            // Configure decimal precision
            modelBuilder.Entity<Salary>()
                .Property(s => s.BasicSalary)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Salary>()
                .Property(s => s.Allowance)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Salary>()
                .Property(s => s.Bonus)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Salary>()
                .Property(s => s.OvertimePay)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Salary>()
                .Property(s => s.Deductions)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Salary>()
                .Property(s => s.NetSalary)
                .HasPrecision(15, 2);

            modelBuilder.Entity<Timesheet>()
                .Property(t => t.HoursWorked)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Attendance>()
                .Property(a => a.TotalHours)
                .HasPrecision(5, 2);

            // Configure Project Technology field for SQLite
            modelBuilder.Entity<Project>()
                .Property(p => p.Technology)
                .HasColumnType("TEXT");

            // Configure Attendance Photo fields for SQLite
            modelBuilder.Entity<Attendance>()
                .Property(a => a.CheckInPhoto)
                .HasColumnType("TEXT");

            modelBuilder.Entity<Attendance>()
                .Property(a => a.CheckOutPhoto)
                .HasColumnType("TEXT");
        }
    }
}
