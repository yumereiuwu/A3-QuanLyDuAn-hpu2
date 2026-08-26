using HRManagement.Core.Entities;
using HRManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.API.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(HRManagementDbContext context)
        {
            // Do NOT clear existing data. Only ensure an admin account exists.
            var existingAdmin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");

            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Email = "admin@company.com",
                    FullName = "Administrator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Department = "IT",
                    Position = "System Administrator",
                    Role = "admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
