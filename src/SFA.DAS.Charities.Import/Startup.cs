using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Charities.Data.Extensions;
using SFA.DAS.Charities.Data.Repositories;
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

            AddNLog(builder);

            var connectionString = configuration["SqlDatabaseConnectionString"];
            var environment = configuration["EnvironmentName"];

            builder.Services.AddCharityDataContext(connectionString, environment);
            
            RegisterCharityCommissionsHttpClient(builder, configuration);

            builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddTransient<ICharityImportRepository, ChairtyImportRepository>();
        }

        private static void AddNLog(IFunctionsHostBuilder builder)
        {
            var nLogConfiguration = new NLogConfiguration();

            builder.Services.AddLogging((options) =>
            {
                options.AddFilter(typeof(Startup).Namespace, LogLevel.Information);
                options.SetMinimumLevel(LogLevel.Trace);
                options.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
                options.AddConsole();

                nLogConfiguration.ConfigureNLog();
            });
        }

        private static IConfiguration BuildConfiguration(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddAzureTableStorage(options => 
                {
                    options.ConfigurationKeys = new[] { "SFA.DAS.Charities.Import.Functions" };
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                })
                .Build();
            builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
            return config;
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
