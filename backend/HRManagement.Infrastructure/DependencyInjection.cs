using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using HRManagement.Infrastructure.Repositories;
using HRManagement.Infrastructure.Services;
using HRManagement.Core.Interfaces;
using HRManagement.Infrastructure.Data;

namespace HRManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Entity Framework configuration
            services.AddDbContext<HRManagementDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ICertificateRepository, CertificateRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITimesheetRepository, TimesheetRepository>();
            services.AddScoped<ISalaryRepository, SalaryRepository>();
            services.AddScoped<IPerformanceReviewRepository, PerformanceReviewRepository>();

            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
