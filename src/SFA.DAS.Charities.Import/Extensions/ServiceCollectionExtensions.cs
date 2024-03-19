using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;

namespace SFA.DAS.Charities.Import.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCharityCommissionsHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        var charityFilesDownloadUrl = configuration["CharitiesFilesDownloadUrl"];

        if (string.IsNullOrWhiteSpace(charityFilesDownloadUrl))
        {
            throw new Exception("CharityFilesDownloadUrl not set in the configuration");
        }

        services
            .AddHttpClient("CharityCommissions", c => c.BaseAddress = new Uri(charityFilesDownloadUrl))
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(retryPolicy);

        return services;
    }
}
