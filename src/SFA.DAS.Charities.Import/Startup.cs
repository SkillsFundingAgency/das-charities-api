using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Infrastructure;
using SFA.DAS.Configuration.AzureTableStorage;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SFA.DAS.Charities.Import.Functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = BuildConfiguration(builder);
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), configuration));
            RegisterCharityCommissionsHttpClient(builder, configuration);

            builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }

        private static IConfiguration BuildConfiguration(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            return new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorage(options => 
                {
                    options.ConfigurationKeys = new[] { "SFA.DAS.Charities.Import.Functions" };
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                })
                .Build();
        }

        private static void RegisterCharityCommissionsHttpClient(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            var charityFilesDownloadUrl = configuration["CharitiesFilesDownloadUrl"];

            if (string.IsNullOrWhiteSpace(charityFilesDownloadUrl))
            {
                throw new Exception("CharityFilesDownloadUrl not set in the configuration");
            }

            builder.Services
                .AddHttpClient("CharityCommissions", c => c.BaseAddress = new Uri(charityFilesDownloadUrl))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(retryPolicy);
        }
    }
}
