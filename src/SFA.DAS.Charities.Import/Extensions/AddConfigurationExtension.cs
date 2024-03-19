using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Charities.Import.Extensions;

public static class AddConfigurationExtension
{
    public static void AddConfiguration(this IConfigurationBuilder builder)
    {
        //var configuration = BuildConfiguration(builder);

        var configuration = builder.Build();
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

        // builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));


        //AddNLog(builder);

        //var connectionString = configuration["SqlDatabaseConnectionString"];
        //var environment = configuration["EnvironmentName"];

    }

    // private static void AddNLog(IConfigurationBuilder builder)
    // {
    //     var nLogConfiguration = new NLogConfiguration();
    //
    //     builder.Services.AddLogging((options) =>
    //     {
    //         options.AddFilter(typeof(Program).Namespace, LogLevel.Information);
    //         options.SetMinimumLevel(LogLevel.Trace);
    //         options.AddNLog(new NLogProviderOptions
    //         {
    //             CaptureMessageTemplates = true,
    //             CaptureMessageProperties = true
    //         });
    //         options.AddConsole();
    //
    //         nLogConfiguration.ConfigureNLog();
    //     });
    // }

    // private static IConfiguration BuildConfiguration(IConfigurationBuilder builder)
    //     {
    //         var configuration = builder.Build();
    //         var config = new ConfigurationBuilder()
    //             .AddConfiguration(configuration)
    //             .AddAzureTableStorage(options =>
    //             {
    //                 options.ConfigurationKeys = new[] { "SFA.DAS.Charities.Import.Functions" };
    //                 options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
    //                 options.EnvironmentName = configuration["EnvironmentName"];
    //                 options.PreFixConfigurationKeys = false;
    //             })
    //             .Build();
    //         builder.Services.Replace(ServiceDescriptor.Singleton(typeof(IConfiguration), config));
    //
    //         return config;
    //     }
}

