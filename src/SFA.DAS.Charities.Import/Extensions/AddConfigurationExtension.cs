using System;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Charities.Import.Extensions;

public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IConfigurationBuilder builder)
    {
        IConfiguration configuration = builder.Build();

        builder
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
}
