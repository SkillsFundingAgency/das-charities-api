using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Charities.Import.Services;

namespace SFA.DAS.Charities.Import.Extensions;

[ExcludeFromCodeCoverage]

public static class AddServiceRegistrationsExtension
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .RegisterServices()
            .AddHttpClient()
            .RegisterCharityCommissionsHttpClient(configuration);
    }

    private static IServiceCollection RegisterCharityCommissionsHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var charityFilesDownloadUrl = configuration["CharitiesFilesDownloadUrl"];

        if (string.IsNullOrWhiteSpace(charityFilesDownloadUrl))
        {
            throw new InvalidOperationException("CharityFilesDownloadUrl not set in the configuration");
        }

        services
            .AddHttpClient("CharityCommissions", c => c.BaseAddress = new Uri(charityFilesDownloadUrl))
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(retryPolicy);

        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<ICharitiesImportRepository, CharitiesImportRepository>();
        services.AddTransient<ICharityCommissionDataHelper, CharityCommissionDataHelper>();
        services.AddTransient<IDurableTaskClientWrapper, DurableTaskClientWrapper>();
        return services;
    }
}
