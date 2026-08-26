using Microsoft.Extensions.DependencyInjection;

namespace HRManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Add application services here
            // For now, we'll keep it simple as most logic is in Infrastructure
            
            return services;
        }
    }
}
