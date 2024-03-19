using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Import.Infrastructure;

namespace SFA.DAS.Charities.Import.Extensions;
public static class AddApplicationRegistrationsExtension
{
    public static IServiceCollection AddApplicationRegistrations(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<ICharitiesImportRepository, CharitiesImportRepository>();

        return services;
    }
}
