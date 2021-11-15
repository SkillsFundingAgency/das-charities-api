using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.Charities.Import.Functions;
using SFA.DAS.Charities.Import.Infrastructure;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SFA.DAS.Charities.Import.Functions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            RegisterCharityCommissionsHttpClient(builder, configuration);

            builder.Services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        }

        private static void RegisterCharityCommissionsHttpClient(IFunctionsHostBuilder builder, IConfiguration configuration)
        {
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError().WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            var charityFilesDownloadUrl = configuration["CharityFilesDownloadUrl"];
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
