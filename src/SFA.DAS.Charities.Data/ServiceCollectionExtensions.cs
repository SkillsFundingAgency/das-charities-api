using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Charities.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CharitiesDataContext>(options => options.UseSqlServer(connectionString));
            return services;
        }
    }
}
